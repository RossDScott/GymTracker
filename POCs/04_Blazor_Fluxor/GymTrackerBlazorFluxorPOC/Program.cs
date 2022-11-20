using Fluxor;
using GymTrackerBlazorFluxorPOC;
using GymTrackerBlazorFluxorPOC.Session.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly).UseReduxDevTools());

//builder.Services.AddScoped<SessionData>();
builder.Services.AddScoped<SessionService>();

await builder.Build().RunAsync();
