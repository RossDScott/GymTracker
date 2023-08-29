using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

public static class WorkoutReducers
{
    [ReducerMethod]
    public static WorkoutState OnSetWorkout(WorkoutState state, SetWorkoutAction action)
        => state with { Workout = action.workout.ToWorkoutDetail() };
}
