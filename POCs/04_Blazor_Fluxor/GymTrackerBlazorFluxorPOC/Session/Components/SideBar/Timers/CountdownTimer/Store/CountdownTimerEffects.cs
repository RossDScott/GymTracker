using Fluxor;
using System.Timers;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Timers.CountdownTimer.Store;

public class CountdownTimerEffects
{
    private System.Timers.Timer _timer = new(1);
    private readonly IState<CountdownTimerState> _state;
    private IDispatcher _dispatcher;

    public CountdownTimerEffects(IState<CountdownTimerState> state, IDispatcher dispatcher)
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

        var duration = currentState.StartTime.Value.Subtract(DateTime.Now).Add(currentState.PausedDuration ?? currentState.StartDuration);

        _dispatcher.Dispatch(new CountdownTimerSetDurationAction(Duration: duration));

        if (duration.TotalMilliseconds <= 0)
        {
            _dispatcher.Dispatch(new CountdownTimerTimesUpAction());

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                _dispatcher.Dispatch(new CountdownTimerResetAction());
            });
        }
    }

    [EffectMethod]
    public async Task OnStart(CountdownTimerStartAction action, IDispatcher dispatcher)
    {
        _timer.Start();
    }

    [EffectMethod]
    public async Task OnPause(CountdownTimerPauseAction action, IDispatcher dispatcher)
    {
        _timer.Stop();
    }

    [EffectMethod]
    public async Task OnReset(CountdownTimerResetAction action, IDispatcher dispatcher)
    {
        _timer.Stop();
    }
}
