using Fluxor;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

[FeatureState]
public record AppSettingsState
{
    public Models.ClientStorage.AppSettings Settings { get; init; } = default!;

    public ImmutableList<string> BodyTargets { get; set; } = ImmutableList<string>.Empty!;
    public ImmutableList<string> Equipment { get; set; } = ImmutableList<string>.Empty!;
}
