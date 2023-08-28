using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

[FeatureState]
public record WorkoutState
{
    public bool HasExistingWorkout { get; init; } = false;



}



