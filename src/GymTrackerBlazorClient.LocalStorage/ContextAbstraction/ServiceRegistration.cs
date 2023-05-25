using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.BlazorClient.LocalStorage.ContextAbstraction;
public static class ServiceRegistration
{
    public static void RegisterLocalStorageContext<T>(this IServiceCollection services) where T : LocalStorageContext, new()
    {
        services.AddBlazoredLocalStorage();
        services.AddScoped<T>(serviceProvider =>
        {
            var localStorage = serviceProvider.GetRequiredService<ILocalStorageService>();
            var contextType = typeof(T);

            var context = new T();

            context.Initialise(localStorage);

            return context;
        });
    }


}
