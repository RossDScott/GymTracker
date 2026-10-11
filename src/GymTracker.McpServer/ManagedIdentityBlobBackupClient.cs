using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GymTracker.Domain;

namespace GymTracker.McpServer;

/// <summary>
/// Reads and writes the app's backup blobs using the host's Azure identity (managed identity in Azure,
/// developer credentials locally) instead of a SAS URI. Blob names and content match BlobBackupClient
/// so the Blazor app and the MCP server share the same data.
/// </summary>
public class ManagedIdentityBlobBackupClient : IDataBackupClient
{
    private readonly BlobContainerClient _container;
    private readonly ILogger<ManagedIdentityBlobBackupClient> _logger;

    public ManagedIdentityBlobBackupClient(
        BlobContainerClient container,
        ILogger<ManagedIdentityBlobBackupClient> logger)
    {
        _container = container;
        _logger = logger;
    }

    public Task BackupAsync(string key, string dataAsString)
    {
        if (string.IsNullOrWhiteSpace(dataAsString)) return Task.CompletedTask;
        ArgumentNullException.ThrowIfNull(key);

        return Execute(key, async () =>
        {
            await _container.GetBlobClient(key).UploadAsync(
                BinaryData.FromString(dataAsString),
                new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "text/plain; charset=utf-8" } });
        });
    }

    public Task<string> DownloadBackupItem(string key) =>
        Execute(key, async () =>
        {
            var result = await _container.GetBlobClient(key).DownloadContentAsync();
            return result.Value.Content.ToString();
        });

    public Task<bool> BackupExistsAsync(string key) =>
        Execute(key, async () => (bool)await _container.GetBlobClient(key).ExistsAsync());

    public Task DeleteAsync(string key) =>
        Execute(key, async () => { await _container.GetBlobClient(key).DeleteIfExistsAsync(); });

    public Task<bool> IsConfiguredAsync() => Task.FromResult(true);

    // Logs the full storage error server-side but only surfaces a generic message, so storage
    // account details and SDK diagnostics are never echoed back to the MCP client.
    private async Task<T> Execute<T>(string key, Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (RequestFailedException ex)
        {
            var errorId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogError(ex, "Blob storage request for {BlobName} failed (error id {ErrorId})", key, errorId);
            throw new InvalidOperationException(
                $"Storage request for '{key}' failed with status {ex.Status}. Error id: {errorId}.");
        }
    }

    private Task Execute(string key, Func<Task> action) =>
        Execute(key, async () => { await action(); return true; });
}

public class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Blob service endpoint, e.g. https://myaccount.blob.core.windows.net</summary>
    public Uri BlobServiceUri { get; set; } = null!;

    /// <summary>Name of the container holding the app's backup blobs.</summary>
    public string ContainerName { get; set; } = "";
}
