using Blazored.LocalStorage;
using GymTracker.Domain;
using GymTracker.LocalStorage.Core;
using GymTracker.LocalStorage.IndexedDb;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.LocalStorage;
public static class ServiceRegistration
{
    public static void RegisterLocalStorageContext<TService, TImplementation>
        (this IServiceCollection services)
        where TImplementation : LocalStorageContext, TService, new()
        where TService : class
    {
        // Keep Blazored.LocalStorage for data migration from localStorage to IndexedDB
        services.AddBlazoredLocalStorage();

        services.AddScoped<IIndexedDbService, IndexedDbService>();

        services.AddScoped<TImplementation>(serviceProvider =>
        {
            var indexedDb = serviceProvider.GetRequiredService<IIndexedDbService>();

            var context = new TImplementation();

            context.Initialise(indexedDb, serviceProvider);

            return context;
        });
        services.AddScoped<TService>(serviceProvider
            => serviceProvider.GetRequiredService<TImplementation>());

        services.AddScoped<IBackupOrchestrator, BackupOrchestrator>();
        services.AddScoped<IAppSettingsProvider, AppSettingsProvider>();
        services.AddScoped<StorageDiagnosticsService>();
    }
}
