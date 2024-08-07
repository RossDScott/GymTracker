﻿namespace GymTracker.BlazorClient.Features.Workout.End.Store;

using Models = GymTracker.Domain.Models;

public record EndWorkoutAction();
public record ConfirmEndWorkoutAction();
public record CancelEndWorkoutAction();
public record SetEndWorkoutAction(Models.Workout Workout, Models.Statistics.WorkoutPlanStatistics? PreviousStatistics);
public record SetSelectedProgressAction(Guid workoutExerciseId, ProgressType progressType);