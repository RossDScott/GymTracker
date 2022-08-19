using GymTrackerBlazorPOC;
using GymTrackerBlazorPOC.Session;
using GymTrackerBlazorPOC.Session.SideBar.Timers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<SessionData>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<CountdownTimerService>();

await builder.Build().RunAsync();
