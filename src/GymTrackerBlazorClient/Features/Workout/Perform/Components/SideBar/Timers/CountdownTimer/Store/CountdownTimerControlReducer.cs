﻿using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.SideBar.Timers.CountdownTimer.Store;

public static class CountdownTimerControlReducer
{
    [ReducerMethod]
    public static CountdownTimerState OnStart(CountdownTimerState state, CountdownTimerStartAction action)
    {
        return state with
        {
            StartTime = action.StartTime
        };
    }

    [ReducerMethod]
    public static CountdownTimerState OnPause(CountdownTimerState state, CountdownTimerPauseAction action)
    {
        return state with
        {
            PausedDuration = state.Duration,
            StartTime = null
        };
    }

    [ReducerMethod]
    public static CountdownTimerState OnReset(CountdownTimerState state, CountdownTimerResetAction action)
    {
        return state with
        {
            StartTime = null,
            PausedDuration = null,
            Duration = TimeSpan.FromMilliseconds(0),
            TimesUp = false
        };
    }

    [ReducerMethod]
    public static CountdownTimerState OnTimesUp(CountdownTimerState state, CountdownTimerTimesUpAction action)
    {
        return state with
        {
            TimesUp = true,
            StartTime = null,
            PausedDuration = null,
            Duration = TimeSpan.FromMilliseconds(0)
        };
    }
}
