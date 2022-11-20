using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Actions;

namespace GymTrackerBlazorFluxorPOC.Session.Reducers;

public static class SessionReducer
{
    [ReducerMethod]
    public static SessionState OnSetupWorkoutSession(SessionState state, SetSessionAction action)
    {
        return new SessionState
        {
            Session = action.Session,
            SelectedExercise = null
        };
    }

    [ReducerMethod]
    public static SessionState OnSetSelectedExercise(SessionState state, SetSelectedExerciseAction action)
    {
        return state with { SelectedExercise = action.Exercise };
    }
}
