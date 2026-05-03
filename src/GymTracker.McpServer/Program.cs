using GymTracker.AzureBlobStorage;
using GymTracker.Domain;
using GymTracker.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;

// Redirect Console.Out to stderr so any startup text doesn't corrupt the JSON-RPC stream.
// The MCP stdio transport uses Console.OpenStandardOutput() directly, so it is unaffected.
Console.SetOut(Console.Error);

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddSingleton<IAppSettingsProvider, EnvironmentAppSettingsProvider>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IDataBackupClient, BlobBackupClient>();
builder.Services.AddSingleton<GymDataService>();

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "GymTracker",
            Version = "1.0.0"
        };
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
