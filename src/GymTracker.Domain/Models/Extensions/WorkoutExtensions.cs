using GymTracker.Domain.Models.Statistics;

namespace GymTracker.Domain.Models.Extensions;
public static class WorkoutExtensions
{
    public static WorkoutStatistics ToWorkoutStatistics(this Workout workout)
        => new WorkoutStatistics
        {
            WorkoutId = workout.Id,
            CompletedOn = workout.WorkoutEnd!.Value,
            TotalWeightVolume = workout.GetWeightTotalVolume(),
            TotalWeightVolumeWithMeasure = workout.GetWeightTotalVolumeWithMeasure()
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
