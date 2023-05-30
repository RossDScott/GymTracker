using Fluxor;

namespace GymTracker.BlazorClient.Features.Settings.Store;

[FeatureState]
public record SettingsState
{
    public string AzureBlobBackupContainerSASURI { get; init; }
}
