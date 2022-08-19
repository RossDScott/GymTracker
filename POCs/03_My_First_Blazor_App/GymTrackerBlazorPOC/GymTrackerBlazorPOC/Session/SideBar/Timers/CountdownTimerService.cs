namespace GymTrackerBlazorPOC.Session.SideBar.Timers;

public class CountdownTimerService
{
    public event Action? OnStartDurationChange;
    public event Action? OnStart;

    public TimeSpan StartDuration { get; private set; } = TimeSpan.FromSeconds(2);
    public void SetStartDuration(TimeSpan startDuration)
    {
        StartDuration = startDuration;
        OnStartDurationChange?.Invoke();
    }

    public void StartTimer() => OnStart?.Invoke();
    public void StartTimer(TimeSpan duration)
    {
        SetStartDuration(duration);
        OnStart?.Invoke();
    }
}
