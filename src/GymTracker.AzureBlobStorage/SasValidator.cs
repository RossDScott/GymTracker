using System.Diagnostics;

namespace GymTracker.AzureBlobStorage;

public class SasValidator : ISasValidator
{
    private readonly HttpClient _http;

    public SasValidator(HttpClient http) => _http = http;

    private static string BlobUrl(string containerSasUri, string blobName)
    {
        var uri = new Uri(containerSasUri);
        return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}/{Uri.EscapeDataString(blobName)}{uri.Query}";
    }

    public async Task<IReadOnlyList<SasValidationStepResult>> ValidateAsync(string sasUri)
    {
        var results = new List<SasValidationStepResult>();
        bool created = false;
        var blobUrl = BlobUrl(sasUri, $"sas-validation-{Guid.NewGuid():N}");
        var sw = new Stopwatch();

        try
        {
            // Step 1: Create
            sw.Restart();
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Put, blobUrl);
                req.Headers.Add("x-ms-blob-type", "BlockBlob");
                req.Content = new StringContent("gymtracker-validation", System.Text.Encoding.UTF8, "text/plain");
                (await _http.SendAsync(req)).EnsureSuccessStatusCode();
                results.Add(new("Create", true, sw.Elapsed));
                created = true;
            }
            catch (Exception ex) { results.Add(new("Create", false, sw.Elapsed, ex)); return results; }

            // Step 2: Update
            sw.Restart();
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Put, blobUrl);
                req.Headers.Add("x-ms-blob-type", "BlockBlob");
                req.Content = new StringContent("gymtracker-validation-updated", System.Text.Encoding.UTF8, "text/plain");
                (await _http.SendAsync(req)).EnsureSuccessStatusCode();
                results.Add(new("Update", true, sw.Elapsed));
            }
            catch (Exception ex) { results.Add(new("Update", false, sw.Elapsed, ex)); return results; }

            // Step 3: Read
            sw.Restart();
            try
            {
                var resp = await _http.GetAsync(blobUrl);
                resp.EnsureSuccessStatusCode();
                _ = await resp.Content.ReadAsStringAsync();
                results.Add(new("Read", true, sw.Elapsed));
            }
            catch (Exception ex) { results.Add(new("Read", false, sw.Elapsed, ex)); return results; }

            // Step 4: Delete
            sw.Restart();
            try
            {
                (await _http.DeleteAsync(blobUrl)).EnsureSuccessStatusCode();
                created = false;
                results.Add(new("Delete", true, sw.Elapsed));
            }
            catch (Exception ex) { results.Add(new("Delete", false, sw.Elapsed, ex)); }
        }
        catch (Exception ex) { results.Add(new("Overall", false, sw.Elapsed, ex)); }
        finally
        {
            if (created)
                try { await _http.DeleteAsync(blobUrl); } catch { }
        }

        return results;
    }
}
