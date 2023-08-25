using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Store;

public record FetchWorkoutPlansAction(WorkoutListOrder Order);
public record SetWorkoutPlansAction(ImmutableArray<WorkoutPlanListItem> WorkoutPlans);
