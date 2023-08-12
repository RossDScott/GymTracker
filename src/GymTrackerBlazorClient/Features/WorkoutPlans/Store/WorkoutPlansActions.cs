using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public record FetchWorkoutPlansInitialStateAction();
public record SetWorkoutPlansAction(IEnumerable<WorkoutPlan> WorkoutPlans);
public record FetchWorkoutPlanAction(Guid Id);
public record SetWorkoutPlanAction(WorkoutPlan WorkoutPlan);
public record AddOrUpdateWorkoutPlanAction(WorkoutPlanDetail WorkoutPlan);
public record AddExerciseToWorkoutPlan(Guid WorkoutPlanId, Guid ExerciseId);
public record CreateNewWorkoutPlanAction();
public record NavigateToNewWorkoutPlanAction(Guid Id);
public record FetchExerciseAction(Guid WorkoutPlanId, Guid ExerciseId);
public record SetExerciseAction(PlannedExercise Exercise);
public record ChangeOrderAction(Guid WorkoutPlanId, Guid ExerciseId, OrderDirection Direction);