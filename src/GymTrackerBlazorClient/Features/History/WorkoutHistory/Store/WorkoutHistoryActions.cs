namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

using GymTracker.Domain.Models;
using MudBlazor;
using Models = GymTracker.Domain.Models;

public record InitialiseAction();
public record SetInitialDataAction(
    IEnumerable<WorkoutPlan> WorkoutPlans,
    IEnumerable<Models.Workout> Workouts);
public record ViewWorkoutHistoryAction(Guid WorkoutPlanId);
public record SetWorkoutPlanIdAction(Guid Id);
public record SetDateRangeAction(DateRange DateRange);
public record SetPageAction(int Page);
