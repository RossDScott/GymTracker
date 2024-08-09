using GymTracker.Domain.Models.Statistics;
using System.Collections.Immutable;

namespace GymTracker.Domain.Models.Extensions;
public static class WorkoutExtensions
{
    public static WorkoutStatistics ToWorkoutStatistics(this Workout workout)
        => new WorkoutStatistics
        {
            WorkoutId = workout.Id,
            WorkoutPlanId = workout.Plan.Id,
            WorkoutPlanName = workout.Plan.Name,
            CompletedOn = workout.WorkoutEnd!.Value,
            TotalTime = workout.WorkoutEnd.Value - workout.WorkoutStart,
            TotalWeightVolume = workout.GetWeightTotalVolume(),
            TotalWeightVolumeWithMeasure = workout.GetWeightTotalVolumeWithMeasure(),
            Exercises = workout.Exercises
                               .Select(x => new WorkoutExerciseStatistics
                               {
                                   ExerciseName = x.Exercise.Name,
                                   MetricType = x.Exercise.MetricType,
                                   MaxSet = x.Sets.Select(s => s.Metrics)
                                                  .GetMaxSet(x.Exercise.MetricType)
                               }).ToImmutableArray()

        };

    public static decimal GetWeightTotalVolume(this Workout workout)
        => workout
            .Exercises
            .Where(x => x.Exercise.MetricType == MetricType.Weight)
            .SelectMany(x => x.Sets)
            .Select(x => x.Metrics)
            .GetWeightTotalVolume();

    public static string GetWeightTotalVolumeWithMeasure(this Workout workout)
        => workout
            .Exercises
            .Where(x => x.Exercise.MetricType == MetricType.Weight)
            .SelectMany(x => x.Sets)
            .Select(x => x.Metrics)
            .GetWeightTotalVolumeWithMeasure();
}
