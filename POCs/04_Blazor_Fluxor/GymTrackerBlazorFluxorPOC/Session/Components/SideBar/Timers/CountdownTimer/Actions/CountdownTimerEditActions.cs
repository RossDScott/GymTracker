namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Timers.CountdownTimer.Actions;

public record CountdownTimerStartEditAction { }
public record CountdownTimerSetDurationAction(TimeSpan Duration);
public record CountdownTimerSetStartDurationAction(TimeSpan StartDuration);
