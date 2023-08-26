using System.Collections.Immutable;

namespace GymTracker.Domain.Models;
public record AppSettings
{
    public string? AzureBlobBackupContainerSASURI { get; init; } = null;

    public ImmutableArray<string> TargetBodyParts { get; init; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> Equipment { get; init; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> SetType { get; init; } = ImmutableArray<string>.Empty;
}