using Azure.Storage.Blobs;
using System.Diagnostics;

namespace GymTracker.AzureBlobStorage;

public class SasValidator : ISasValidator
{
    public async Task<IReadOnlyList<SasValidationStepResult>> ValidateAsync(string sasUri)
    {
        var results = new List<SasValidationStepResult>();
        var blobName = $"sas-validation-{Guid.NewGuid():N}";
        var containerClient = new BlobContainerClient(new Uri(sasUri));
        var blobClient = containerClient.GetBlobClient(blobName);
        bool created = false;
        var sw = new Stopwatch();

        try
        {
            // Step 1: Create
            sw.Restart();
            try
            {
                await blobClient.UploadAsync(BinaryData.FromString("gymtracker-validation"), overwrite: false);
                results.Add(new("Create", true, sw.Elapsed));
                created = true;
            }
            catch (Exception ex) { results.Add(new("Create", false, sw.Elapsed, ex)); return results; }

            // Step 2: Update
            sw.Restart();
            try
            {
                await blobClient.UploadAsync(BinaryData.FromString("gymtracker-validation-updated"), overwrite: true);
                results.Add(new("Update", true, sw.Elapsed));
            }
            catch (Exception ex) { results.Add(new("Update", false, sw.Elapsed, ex)); return results; }

            // Step 3: Read
            sw.Restart();
            try
            {
                var download = await blobClient.DownloadContentAsync();
                _ = download.Value.Content.ToString();
                results.Add(new("Read", true, sw.Elapsed));
            }
            catch (Exception ex) { results.Add(new("Read", false, sw.Elapsed, ex)); return results; }

            // Step 4: Delete
            sw.Restart();
            try
            {
                await blobClient.DeleteAsync();
                created = false;
                results.Add(new("Delete", true, sw.Elapsed));
            }
            catch (Exception ex) { results.Add(new("Delete", false, sw.Elapsed, ex)); }
        }
        finally
        {
            if (created)
                try { await blobClient.DeleteIfExistsAsync(); } catch { }
        }

        return results;
    }
}
