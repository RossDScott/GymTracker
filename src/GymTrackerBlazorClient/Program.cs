using Fluxor;
using GymTracker.AzureBlobStorage;
using GymTracker.BlazorClient;
using GymTracker.BlazorClient.Features.SidePanel;
using GymTracker.Domain;
using GymTracker.LocalStorage;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.RegisterLocalStorageContext<IClientStorage, ClientStorageContext>();

builder.Services.RegisterDomainServices();
builder.Services.RegisterAzureBlobStorageServices();

builder.Services.AddScoped<SidePanelService>();

var serviceProvider = builder.Services.BuildServiceProvider();
//await serviceProvider.ConfigureAzureBlobBackupSettings();
//var exerciseLogService = serviceProvider.GetRequiredService<ExerciseLogService>();

builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly));

await builder.Build().RunAsync();
