using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services;
using GymTracker.LocalStorage.ContextAbstraction;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.LocalStorage;
public static class ServiceRegistration
{
    public static void RegisterLocalStorageContext<TService, TImplementation>
        (this IServiceCollection services) 
        where TImplementation : LocalStorageContext, TService, new()
        where TService : class
    {
        services.AddBlazoredLocalStorage();
        services.AddScoped<TService>(serviceProvider =>
        {
            var localStorage = serviceProvider.GetRequiredService<ILocalStorageService>();

            var context = new TImplementation();

            context.Initialise(localStorage);

            return context;
        });
    }
}
