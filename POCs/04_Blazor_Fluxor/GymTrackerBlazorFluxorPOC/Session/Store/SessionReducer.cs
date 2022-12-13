using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session.Store;

public static class SessionReducer
{
    [ReducerMethod]
    public static SessionState OnSetupWorkoutSession(SessionState state, SetSessionAction action)
    {
        return new SessionState
        {
            Session = action.Session,
            SelectedSessionExercise = null
        };
    }

    [ReducerMethod]
    public static SessionState OnSetSelectedExercise(SessionState state, SetSelectedExerciseAction action)
    {
        return state with { SelectedSessionExercise = action.SessionExercise };
    }
}
