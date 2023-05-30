using Azure.Storage.Blobs;
using GymTracker.Domain.Abstractions.Services;

namespace GymTracker.AzureBlobStorage;
public class BlobBackupService : IDataBackupService
{
    private AzureBlobBackupSettings? _azureBlobBackupSettings;

    public void Configure(AzureBlobBackupSettings azureBlobBackupSettings)
    {
        _azureBlobBackupSettings = azureBlobBackupSettings;
    }

    public async Task BackupAsync(object? data, string name)
    {
        if (data == null || string.IsNullOrWhiteSpace(_azureBlobBackupSettings?.ContainerSASURI))
            return;

        var containerClient = new BlobContainerClient(new Uri(_azureBlobBackupSettings.ContainerSASURI));

        var blob = containerClient.GetBlobClient(name);
        await blob.UploadAsync(BinaryData.FromObjectAsJson(data), overwrite: true);
    }

    public async Task RestoreAllAsync()
    {
        await Task.CompletedTask;
    }
}
