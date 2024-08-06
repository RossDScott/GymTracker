using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GymTracker.Domain;

namespace GymTracker.AzureBlobStorage;
public class BlobBackupClient : IDataBackupClient
{
    private readonly IAppSettingsProvider _appSettings;

    public BlobBackupClient(IAppSettingsProvider appSettings)
    {
        _appSettings = appSettings;
    }

    private async ValueTask<BlobContainerClient?> BuildContainerClient()
    {
        var sasUri = (await _appSettings.GetAsync()).AzureBlobBackupContainerSASURI;

        return string.IsNullOrWhiteSpace(sasUri)
            ? null
            : new BlobContainerClient(new Uri(sasUri));
    }

    public async Task BackupAsync(string key, string dataAsString)
    {
        var containerClient = await BuildContainerClient();
        if (string.IsNullOrWhiteSpace(dataAsString) || containerClient is null)
            return;

        ArgumentNullException.ThrowIfNull(key);

        var blob = containerClient.GetBlobClient(key);
        await blob.UploadAsync(BinaryData.FromString(dataAsString), overwrite: true);
    }

    public async Task<string> DownloadBackupItem(string key)
    {
        var containerClient = await BuildContainerClient();
        ArgumentNullException.ThrowIfNull(containerClient);

        var blob = containerClient.GetBlobClient(key);
        BlobDownloadResult downloadResult = await blob.DownloadContentAsync();
        string blobData = downloadResult.Content.ToString();
        return blobData;
    }

    public async Task<bool> BackupExistsAsync(string key)
    {
        var containerClient = await BuildContainerClient();
        ArgumentNullException.ThrowIfNull(containerClient);

        var blob = containerClient.GetBlobClient(key);
        return await blob.ExistsAsync();
    }
}
