﻿using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public record FetchWorkoutPlansInitialStateAction();
public record SetWorkoutPlansAction(IEnumerable<WorkoutPlan> WorkoutPlans);
public record FetchWorkoutPlanAction(Guid Id);
public record SetWorkoutPlanAction(WorkoutPlan? WorkoutPlan);
public record UpsertWorkoutPlanAction(WorkoutPlanDetail WorkoutPlan);
public record CreateNewWorkoutPlanAction();
public record AddExerciseToWorkoutPlanAction(Guid WorkoutPlanId, Guid ExerciseId);
public record UpdateExerciseForWorkoutPlanAction(Guid WorkoutPlanId, PlannedExerciseEditDTO EditDTO);
public record FetchExerciseAction(Guid WorkoutPlanId, Guid ExerciseId);
public record SetExerciseAction(PlannedExercise Exercise);
public record ChangeOrderAction(Guid WorkoutPlanId, Guid ExerciseId, OrderDirection Direction);
public record RemoveExerciseFromWorkoutAction(Guid WorkoutPlanId, Guid ExerciseId);
public record AddSetToExerciseAction(Guid WorkoutPlanId, PlannedExerciseDetail ExerciseDetail);