using System.Collections.Immutable;
using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Actions;
using GymTrackerBlazorFluxorPOC.Session.SideBar.Exercises.Actions;

namespace GymTrackerBlazorFluxorPOC.Session.SideBar.Exercises.Reducers;

public static class ExercisesReducer
{
    [ReducerMethod]
    public static ExercisesState OnSetExercises(ExercisesState state, SetExercisesAction action)
    {
        return state with
        {
            Exercises = action.Exercises.ToImmutableList()
        };
    }

    [ReducerMethod]
    public static ExercisesState OnSetSelectedExercise(ExercisesState state, SetSelectedExerciseAction action)
    {
        return state with { SelectedExerciseId = action.exerciseId };
    }
}
