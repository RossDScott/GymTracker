using GymTracker.Domain;
using GymTracker.BlazorClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Fluxor;
using GymTracker.AzureBlobStorage;
using GymTracker.LocalStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.RegisterLocalStorageContext<IClientStorage, ClientStorageContext>();

builder.Services.RegisterDomainServices();
builder.Services.RegisterAzureBlobStorageServices();

var serviceProvider = builder.Services.BuildServiceProvider();
//await serviceProvider.ConfigureAzureBlobBackupSettings();

builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly).UseReduxDevTools());

await builder.Build().RunAsync();
