using GymTracker.Domain;
using GymTracker.Domain.Models;

namespace GymTracker.McpServer;

public class EnvironmentAppSettingsProvider : IAppSettingsProvider
{
    public ValueTask<AppSettings> GetAsync()
    {
        var sasUri = Environment.GetEnvironmentVariable("GYMTRACKER_BLOB_SAS_URI")
            ?? throw new InvalidOperationException(
                "GYMTRACKER_BLOB_SAS_URI environment variable is not set.");

        return ValueTask.FromResult(new AppSettings
        {
            AzureBlobBackupContainerSASURI = sasUri
        });
    }
}
