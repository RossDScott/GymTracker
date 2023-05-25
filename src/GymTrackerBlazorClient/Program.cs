using GymTracker.Domain;
using GymTracker.Domain.LocalStorage;
using GymTracker.BlazorClient;
using GymTracker.BlazorClient.LocalStorage.ContextAbstraction;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Fluxor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.RegisterLocalStorageContext<GymTrackerLocalStorageContext>();

builder.Services.RegisterDomainServices();

builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly).UseReduxDevTools());

await builder.Build().RunAsync();
