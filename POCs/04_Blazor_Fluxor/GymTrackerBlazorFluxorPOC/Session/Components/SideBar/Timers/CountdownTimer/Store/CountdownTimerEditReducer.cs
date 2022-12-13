using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Timers.CountdownTimer.Store;

public static class CountdownTimerEditReducer
{
    [ReducerMethod]
    public static CountdownTimerState OnEdit(CountdownTimerState state, CountdownTimerStartEditAction action)
    {
        return state with
        {
            IsEditing = true
        };
    }

    [ReducerMethod]
    public static CountdownTimerState OnSetDuration(CountdownTimerState state, CountdownTimerSetDurationAction action)
    {
        return state with
        {
            Duration = action.Duration
        };
    }

    [ReducerMethod]
    public static CountdownTimerState OnSetStartDuration(CountdownTimerState state, CountdownTimerSetStartDurationAction action)
    {
        return state with
        {
            StartDuration = action.StartDuration,
            IsEditing = false
        };
    }
}
