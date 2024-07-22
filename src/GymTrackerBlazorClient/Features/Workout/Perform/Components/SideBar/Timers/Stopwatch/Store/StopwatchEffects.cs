using Fluxor;
using System.Timers;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.SideBar.Timers.Stopwatch.Store;

public class StopwatchEffects
{
    private System.Timers.Timer _timer = new(1);
    private readonly IState<StopwatchState> _state;
    private IDispatcher _dispatcher;

    public StopwatchEffects(IState<StopwatchState> state, IDispatcher dispatcher)
    {
        _state = state;
        _dispatcher = dispatcher;
        _timer.Elapsed += onTimerElapsed;
    }

    private void onTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var currentState = _state.Value;

        if (currentState.StartTime is null)
            return;

        var duration = DateTime.Now.Subtract(currentState.StartTime.Value);

        if (currentState.PausedDuration is not null)
            duration = duration.Add(currentState.PausedDuration.Value);

        _dispatcher.Dispatch(new StopwatchSetDurationAction { Duration = duration });
    }

    [EffectMethod]
    public Task OnStart(StopwatchStartAction action, IDispatcher dispatcher)
    {
        _timer.Start();
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task OnPause(StopwatchPauseAction action, IDispatcher dispatcher)
    {
        _timer.Stop();
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task OnReset(StopwatchResetAction action, IDispatcher dispatcher)
    {
        _timer.Stop();
        return Task.CompletedTask;
    }
}
