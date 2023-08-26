using GymTracker.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.AzureBlobStorage;
public static class ServiceRegistration
{
    public static void RegisterAzureBlobStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IDataBackupClient, BlobBackupClient>();
    }
}