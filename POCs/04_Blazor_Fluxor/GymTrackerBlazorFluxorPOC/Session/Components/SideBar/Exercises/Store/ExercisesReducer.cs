using System.Collections.Immutable;
using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Store;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Store;

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
        return state with { SelectedSessionExerciseId = action.SessionExercise.Id };
    }
}
