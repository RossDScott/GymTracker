using Fluxor;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

[FeatureState]
public record AppSettingsState
{
    public Models.ClientStorage.AppSettings Settings { get; init; } = default!;

    public ImmutableArray<string> BodyTargets { get; set; } = ImmutableArray<string>.Empty!;
    public ImmutableArray<string> Equipment { get; set; } = ImmutableArray<string>.Empty!;
}
