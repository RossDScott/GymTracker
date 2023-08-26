using Blazored.LocalStorage;
using GymTracker.Domain;
using GymTracker.LocalStorage.Core;
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

        services.AddScoped<TImplementation>(serviceProvider =>
        {
            var localStorage = serviceProvider.GetRequiredService<ILocalStorageService>();

            var context = new TImplementation();

            context.Initialise(localStorage);

            return context;
        });
        services.AddScoped<TService>(serviceProvider
            => serviceProvider.GetRequiredService<TImplementation>());

        services.AddScoped<IBackupOrchestrator, BackupOrchestrator>();
        services.AddScoped<IAppSettingsProvider, AppSettingsProvider>();
    }
}
