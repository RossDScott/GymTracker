namespace GymTrackerBlazorFluxorPOC.Session.SideBar.Timers.CountdownTimer;

public record CountdownTimerState
{
    public DateTime? StartTime { get; init; } = null;
    public TimeSpan StartDuration { get; init; } = TimeSpan.FromMilliseconds(500);
    public TimeSpan? PausedDuration { get; init; } = null;
    public TimeSpan Duration { get; init; } = TimeSpan.FromMilliseconds(0);
    public bool TimesUp { get; init; } = false;
    public bool IsEditing { get; init; } = false;

    public TimeOnly StartDurationTime => TimeOnly.FromTimeSpan(StartDuration);

    public bool ShowCountdownDuration => !TimesUp && (StartTime is not null || PausedDuration is not null);
    public bool ShowStartTimerFace => StartTime is null && !TimesUp && PausedDuration is null;
    public bool IsPaused => PausedDuration is not null && StartTime is null;
}
