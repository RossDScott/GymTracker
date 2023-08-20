using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public record FetchWorkoutPlansInitialStateAction();
public record SetWorkoutPlansAction(IEnumerable<WorkoutPlan> WorkoutPlans);
public record FetchWorkoutPlanAction(Guid Id);
public record SetWorkoutPlanAction(WorkoutPlan? WorkoutPlan);
public record UpsertWorkoutPlanAction(WorkoutPlanDetail WorkoutPlan);
public record AddExerciseToWorkoutPlanAction(Guid WorkoutPlanId, Guid ExerciseId);
public record UpdateExerciseForWorkoutPlanAction(Guid WorkoutPlanId, PlannedExerciseDetail ExerciseDetail);
public record CreateNewWorkoutPlanAction();
public record FetchExerciseAction(Guid WorkoutPlanId, Guid ExerciseId);
public record SetExerciseAction(PlannedExercise Exercise);
public record ChangeOrderAction(Guid WorkoutPlanId, Guid ExerciseId, OrderDirection Direction);