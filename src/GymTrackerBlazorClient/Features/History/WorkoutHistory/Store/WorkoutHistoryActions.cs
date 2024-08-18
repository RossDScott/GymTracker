namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

using Models = GymTracker.Domain.Models;

public record ViewWorkoutHistoryAction(Guid WorkoutId);
public record SetWorkoutPlanIdAction(Guid Id);
public record SetWorkoutHistoryAction(IEnumerable<Models.Workout> Workouts);
