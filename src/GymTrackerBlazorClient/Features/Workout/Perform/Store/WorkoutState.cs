using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

[FeatureState]
public record WorkoutState
{
    public WorkoutDetail Workout { get; init; } = default!;
    public Guid? SelectedExerciseId { get; init; } = null;
}

public record WorkoutDetail
{
    public Guid Id { get; init; }

    public DateTimeOffset WorkoutStart { get; init; } = DateTimeOffset.Now;
    public DateTimeOffset? WorkoutEnd { get; init; }

    public ImmutableArray<ListItem> ExerciseList { get; init; } = ImmutableArray<ListItem>.Empty;
}
