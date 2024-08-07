﻿namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.SideBar.Timers.CountdownTimer.Store;

public record CountdownTimerStartAction(DateTime StartTime);
public record CountdownTimerPauseAction { }
public record CountdownTimerResetAction { }
public record CountdownTimerTimesUpAction { }

public record CountdownTimerStartEditAction { }
public record CountdownTimerSetDurationAction(TimeSpan Duration);
public record CountdownTimerSetStartDurationAction(TimeSpan StartDuration);
public record CountdownTimerStartWithDurationAction(TimeSpan Duration);