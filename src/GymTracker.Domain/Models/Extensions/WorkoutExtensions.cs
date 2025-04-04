﻿using GymTracker.Domain.Models.Statistics;
using System.Collections.Immutable;

namespace GymTracker.Domain.Models.Extensions;
public static class WorkoutExtensions
{
    public static WorkoutStatistic ToWorkoutStatistics(this Workout workout)
        => new WorkoutStatistic
        {
            WorkoutId = workout.Id,
            WorkoutPlanId = workout.Plan.Id,
            WorkoutPlanName = workout.Plan.Name,
            CompletedOn = workout.WorkoutEnd!.Value,
            TotalTime = workout.WorkoutEnd.Value - workout.WorkoutStart,
            TotalWeightVolume = workout.GetWeightTotalVolume(),
            TotalWeightVolumeWithMeasure = workout.GetWeightTotalVolumeWithMeasure(),
            IsRepsOnly = workout.Exercises.All(x => x.Exercise.MetricType == MetricType.Reps),
            TotalReps = workout.GetTotalReps(),
            Exercises = workout.Exercises
                               .Select(x => new WorkoutExerciseStatistics
                               {
                                   ExerciseName = x.Exercise.Name,
                                   MetricType = x.Exercise.MetricType,
                                   MaxSet = x.Sets.Select(s => s.Metrics)
                                                  .GetMaxSet(x.Exercise.MetricType),
                                   AllCompleted = x.Sets.All(s => s.Completed),
                                   AnyCompleted = x.Sets.Any(s => s.Completed)
                               }).ToImmutableArray()
        };

    public static decimal GetWeightTotalVolume(this Workout workout)
        => workout
            .Exercises
            .Where(x => x.Exercise.MetricType == MetricType.Weight)
            .SelectMany(x => x.Sets)
            .Select(x => x.Metrics)
            .GetTotalVolume(MetricType.Weight);

    public static string GetWeightTotalVolumeWithMeasure(this Workout workout)
        => workout
            .Exercises
            .Where(x => x.Exercise.MetricType == MetricType.Weight)
            .SelectMany(x => x.Sets)
            .Select(x => x.Metrics)
            .GetWeightTotalVolumeWithMeasure();

    public static int GetTotalReps(this Workout workout)
        => workout
            .Exercises
            .Where(x => x.Exercise.MetricType == MetricType.Reps)
            .SelectMany(x => x.Sets)
            .Sum(x => x.Metrics.Reps ?? 0);
}
