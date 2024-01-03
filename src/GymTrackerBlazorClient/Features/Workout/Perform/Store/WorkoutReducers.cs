using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

public static class WorkoutReducers
{
    [ReducerMethod]
    public static WorkoutState OnSetWorkout(WorkoutState state, SetWorkoutAction action)
        => state with
        {
            Workout = action.workout.ToWorkoutDetail(),
            SelectedExerciseId = null // action.workout.Exercises.FirstOrDefault()?.Id
        };

    [ReducerMethod]
    public static WorkoutState OnSetSelectedExercise(WorkoutState state, SetSelectedExerciseAction action)
        => state with { SelectedExerciseId = action.ExerciseId };
}
