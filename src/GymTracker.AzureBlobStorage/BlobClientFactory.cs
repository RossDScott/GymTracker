using Azure.Storage.Blobs;

namespace GymTracker.AzureBlobStorage;

internal static class BlobClientFactory
{
    /// <summary>
    /// Creates BlobClientOptions with distributed tracing disabled.
    /// Required for Blazor WASM on Android where System.Diagnostics.DiagnosticSource
    /// fails to initialize, causing a TypeInitializationException in RequestActivityPolicy.
    /// </summary>
    internal static BlobClientOptions CreateOptions()
    {
        var options = new BlobClientOptions();
        options.Diagnostics.IsDistributedTracingEnabled = false;
        return options;
    }

    internal static BlobContainerClient CreateContainerClient(string sasUri) =>
        new BlobContainerClient(new Uri(sasUri), CreateOptions());
}
