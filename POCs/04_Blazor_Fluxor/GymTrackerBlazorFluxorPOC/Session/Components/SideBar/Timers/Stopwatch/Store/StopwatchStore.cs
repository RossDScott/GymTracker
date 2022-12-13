using Fluxor;
namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Timers.Stopwatch.Store;

[FeatureState]
public record StopwatchState
{
    public DateTime? StartTime { get; init; } = null;
    public TimeSpan Duration { get; init; } = TimeSpan.FromMilliseconds(0);
    public TimeSpan? PausedDuration { get; init; } = null;

    public bool IsPaused => PausedDuration is not null && StartTime is null;
}

public record StopwatchStartAction { }
public record StopwatchPauseAction { }
public record StopwatchResetAction { }
public record StopwatchSetDurationAction { public TimeSpan Duration { get; init; } }

public static class StopwatchReducers
{
    [ReducerMethod(typeof(StopwatchStartAction))]
    public static StopwatchState OnStart(StopwatchState state)
    {
        return state with
        {
            StartTime = DateTime.Now
        };
    }

    [ReducerMethod(typeof(StopwatchPauseAction))]
    public static StopwatchState OnPause(StopwatchState state)
    {
        return state with
        {
            PausedDuration = state.Duration,
            StartTime = null
        };
    }

    [ReducerMethod(typeof(StopwatchResetAction))]
    public static StopwatchState OnReset(StopwatchState state)
    {
        return state with
        {
            StartTime = null,
            PausedDuration = null,
            Duration = TimeSpan.FromMilliseconds(0)
        };
    }

    [ReducerMethod]
    public static StopwatchState OnSetDuration(StopwatchState state, StopwatchSetDurationAction action)
    {
        return state with
        {
            Duration = action.Duration
        };
    }
}
