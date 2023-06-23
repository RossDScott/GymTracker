using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

[FeatureState]
public record WorkoutPlansStore
{
    public ImmutableArray<ListItem> OriginalList { get; set; } = ImmutableArray<ListItem>.Empty;
    public ImmutableArray<ListItem> WorkoutPlans { get; set; } = ImmutableArray<ListItem>.Empty;
}
