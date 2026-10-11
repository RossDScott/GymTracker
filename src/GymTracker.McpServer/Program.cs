using System.Threading.RateLimiting;
using Azure.Identity;
using Azure.Storage.Blobs;
using GymTracker.Domain;
using GymTracker.McpServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var mcpOptions = builder.Configuration.GetSection(McpOptions.SectionName).Get<McpOptions>() ?? new();
var auth0Options = builder.Configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>() ?? new();
var storageOptions = builder.Configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new();
ValidateConfiguration(builder.Environment, mcpOptions, auth0Options, storageOptions);

var allowedSubjects = auth0Options.AllowedSubjects.ToHashSet(StringComparer.Ordinal);

// MCP requests are small JSON-RPC messages; cap bodies well below Kestrel's 30 MB default.
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 1024 * 1024);

// App Service terminates TLS at its front end and forwards the original scheme/client IP.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
});

builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        // Unauthenticated calls get a 401 pointing at the protected resource metadata (RFC 9728),
        // which is how Claude discovers Auth0 and starts the OAuth sign-in.
        o.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.Authority = auth0Options.Issuer;
        o.RequireHttpsMetadata = true;
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = auth0Options.Issuer,
            // Tokens must be minted for this server specifically (RFC 8707 audience binding).
            ValidateAudience = true,
            ValidAudience = mcpOptions.ServerUrl,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuerSigningKey = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            NameClaimType = "sub"
        };
        o.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>()
                    .LogWarning("Access token rejected: {Reason}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    })
    .AddMcp(o =>
    {
        o.ResourceMetadata = new()
        {
            Resource = mcpOptions.ServerUrl,
            ResourceName = "GymTracker",
            AuthorizationServers = { auth0Options.Issuer },
            ScopesSupported = ["openid", "offline_access"]
        };
    });

// Dynamic Client Registration means any Auth0 account could obtain a token for this API,
// so every request must also come from an explicitly allowed user.
builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireAssertion(ctx =>
        {
            var sub = ctx.User.FindFirst("sub")?.Value;
            return sub is not null && allowedSubjects.Contains(sub);
        })
        .Build());

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var partitionKey = context.User.FindFirst("sub")?.Value
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 120,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

builder.Services.AddSingleton(_ => new BlobContainerClient(
    new Uri(storageOptions.BlobServiceUri, storageOptions.ContainerName),
    new DefaultAzureCredential()));
builder.Services.AddSingleton<IDataBackupClient, ManagedIdentityBlobBackupClient>();
builder.Services.AddSingleton<GymDataService>();

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "GymTracker",
            Version = "2.0.0"
        };
    })
    .WithHttpTransport(o => o.Stateless = true)
    .WithToolsFromAssembly();

var app = builder.Build();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapGet("/healthz", () => Results.Ok("Healthy")).AllowAnonymous();
app.MapMcp("/mcp").RequireAuthorization();

app.Run();

static void ValidateConfiguration(
    IHostEnvironment environment, McpOptions mcp, Auth0Options auth0, StorageOptions storage)
{
    var errors = new List<string>();

    if (!Uri.TryCreate(mcp.ServerUrl, UriKind.Absolute, out var serverUri))
        errors.Add("Mcp:ServerUrl must be an absolute URL.");
    else if (serverUri.Scheme != Uri.UriSchemeHttps && !environment.IsDevelopment())
        errors.Add("Mcp:ServerUrl must use https.");

    if (string.IsNullOrWhiteSpace(auth0.Domain) || auth0.Domain.Contains('/'))
        errors.Add("Auth0:Domain must be a host name such as my-tenant.eu.auth0.com.");

    if (auth0.AllowedSubjects.Length == 0 || auth0.AllowedSubjects.Any(string.IsNullOrWhiteSpace))
        errors.Add("Auth0:AllowedSubjects must list at least one Auth0 user id.");

    if (storage.BlobServiceUri is null || storage.BlobServiceUri.Scheme != Uri.UriSchemeHttps)
        errors.Add("Storage:BlobServiceUri must be an https URL.");

    if (string.IsNullOrWhiteSpace(storage.ContainerName))
        errors.Add("Storage:ContainerName is required.");

    if (errors.Count > 0)
        throw new InvalidOperationException("Invalid configuration:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
}
