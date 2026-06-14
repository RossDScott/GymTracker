using GymTracker.Domain;

namespace GymTracker.AzureBlobStorage;
public class BlobBackupClient : IDataBackupClient
{
    private readonly IAppSettingsProvider _appSettings;
    private readonly HttpClient _http;

    public BlobBackupClient(IAppSettingsProvider appSettings, HttpClient http)
    {
        _appSettings = appSettings;
        _http = http;
    }

    private static string BlobUrl(string containerSasUri, string blobName)
    {
        var uri = new Uri(containerSasUri);
        return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}/{Uri.EscapeDataString(blobName)}{uri.Query}";
    }

    // Appends a unique value to the SAS query so blob reads bypass the browser HTTP cache.
    // Azure validates only the signed SAS parameters and ignores unknown query params, so the
    // SAS signature is unaffected.
    private static string NoCacheUrl(string containerSasUri, string blobName)
        => $"{BlobUrl(containerSasUri, blobName)}&_={DateTimeOffset.UtcNow.Ticks}";

    private async ValueTask<string?> GetContainerSasUri()
    {
        var sasUri = (await _appSettings.GetAsync()).AzureBlobBackupContainerSASURI;
        return string.IsNullOrWhiteSpace(sasUri) ? null : sasUri;
    }

    public async Task BackupAsync(string key, string dataAsString)
    {
        if (string.IsNullOrWhiteSpace(dataAsString)) return;
        ArgumentNullException.ThrowIfNull(key);
        var sasUri = await GetContainerSasUri();
        if (sasUri is null) return;

        var request = new HttpRequestMessage(HttpMethod.Put, BlobUrl(sasUri, key));
        request.Headers.Add("x-ms-blob-type", "BlockBlob");
        request.Content = new StringContent(dataAsString, System.Text.Encoding.UTF8, "text/plain");
        (await _http.SendAsync(request)).EnsureSuccessStatusCode();
    }

    public async Task<string> DownloadBackupItem(string key)
    {
        var sasUri = await GetContainerSasUri();
        ArgumentNullException.ThrowIfNull(sasUri);
        var request = new HttpRequestMessage(HttpMethod.Get, NoCacheUrl(sasUri, key));
        request.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true
        };
        request.Headers.Pragma.ParseAdd("no-cache");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool> BackupExistsAsync(string key)
    {
        var sasUri = await GetContainerSasUri();
        ArgumentNullException.ThrowIfNull(sasUri);
        var response = await _http.SendAsync(new HttpRequestMessage(HttpMethod.Head, NoCacheUrl(sasUri, key)));
        return response.IsSuccessStatusCode;
    }

    public async Task DeleteAsync(string key)
    {
        var sasUri = await GetContainerSasUri();
        if (sasUri is null) return;

        var response = await _http.SendAsync(new HttpRequestMessage(HttpMethod.Delete, BlobUrl(sasUri, key)));
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            response.EnsureSuccessStatusCode();
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return await GetContainerSasUri() is not null;
    }
}
