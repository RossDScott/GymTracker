using GymTracker.Domain.Abstractions.Services.Backup;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GymTracker.AzureBlobStorage;
public static class ServiceRegistration
{
    public static void RegisterAzureBlobStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IDataBackupClient, BlobBackupClient>();
    }
}