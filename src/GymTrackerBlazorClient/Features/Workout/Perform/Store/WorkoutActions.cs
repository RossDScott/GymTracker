﻿using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

public record StartWorkoutAction(Guid workoutPlanId);
public record ContinueWorkoutAction();
public record SetWorkoutAction(Models.Workout workout);
public record SetSelectedExerciseAction(Guid ExerciseId);
