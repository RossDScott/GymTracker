using GymTracker.Domain.Abstractions.Services;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GymTracker.AzureBlobStorage;
public static class ServiceRegistration
{
    public static void RegisterAzureBlobStorageServices(this IServiceCollection services)
    {
        var blobBackupService = new BlobBackupService();
        services.AddSingleton<IDataBackupService, BlobBackupService>(x => blobBackupService);
        services.AddSingleton<BlobBackupService>(x => blobBackupService);
    }

    public static async Task ConfigureAzureBlobBackupSettings(this IServiceProvider serviceProvider)
    {
        var backupService = serviceProvider.GetRequiredService<BlobBackupService>();
        var clientStorage = serviceProvider.GetRequiredService<IClientStorage>();

        var appSettings = await clientStorage.AppSettings.GetOrDefaultAsync();
        var blobSettings = new AzureBlobBackupSettings
        {
            ContainerSASURI = appSettings!.AzureBlobBackupContainerSASURI
        };

        backupService.Configure(blobSettings);
    }
}