using Fluxor;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

[FeatureState]
public record AppSettingsState
{
    public string? AzureBlobBackupContainerSASURI { get; init; } = null;

    public ImmutableArray<string> TargetBodyParts { get; init; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> Equipment { get; init; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> SetType { get; init; } = ImmutableArray<string>.Empty;
}
