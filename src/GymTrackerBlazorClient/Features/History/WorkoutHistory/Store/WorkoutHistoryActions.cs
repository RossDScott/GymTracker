﻿namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

using GymTracker.Domain.Models;
using Models = GymTracker.Domain.Models;

public record InitialiseAction();
public record SetInitialDataAction(IEnumerable<WorkoutPlan> WorkoutPlans, Guid WorkoutPlanId);
public record ViewWorkoutHistoryAction(Guid WorkoutId);
public record SetWorkoutPlanIdAction(Guid Id);
public record SetWorkoutHistoryAction(IEnumerable<Models.Workout> Workouts);
public record SetPageAction(int page);
