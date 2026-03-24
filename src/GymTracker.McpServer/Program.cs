using GymTracker.AzureBlobStorage;
using GymTracker.Domain;
using GymTracker.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;

var builder = Host.CreateApplicationBuilder(args);

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
