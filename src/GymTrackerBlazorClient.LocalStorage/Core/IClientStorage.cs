﻿using GymTracker.Domain.Models;

namespace GymTracker.LocalStorage.Core;
public interface IClientStorage
{
    IKeyItem<AppSettings> AppSettings { get; init; }
    IKeyListItem<Exercise> Exercises { get; init; }
    IKeyListItem<WorkoutPlan> WorkoutPlans { get; init; }
    IKeyListItem<Workout> Workouts { get; init; }
    IKeyItem<Workout> CurrentWorkout { get; init; }

    IKeyListItem<ExerciseStatistic> ExerciseStatistics { get; init; }
}
