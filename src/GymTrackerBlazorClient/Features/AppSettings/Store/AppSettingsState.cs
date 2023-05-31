using Fluxor;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

[FeatureState]
public record AppSettingsState
{
    public Models.AppSettings Settings { get; init; } = default!;
}
