namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Timers.CountdownTimer.Actions;

public record CountdownTimerStartAction(DateTime StartTime);
public record CountdownTimerPauseAction { }
public record CountdownTimerResetAction { }
public record CountdownTimerTimesUpAction { }


