using System.Collections.Immutable;

namespace GymTracker.Domain.Models.Statistics;
public record WorkoutPlanStatistics
{
    public required Guid WorkoutPlanId { get; init; }

    public required WorkoutStatistics PreviousWorkout { get; init; }

    public decimal BestWeightTotalVolumeIn6Months { get; init; }

    public required ICollection<WorkoutStatistics> History { get; init; }
}

public record WorkoutStatistics
{
    public required Guid WorkoutId { get; init; }
    public required DateTimeOffset CompletedOn { get; init; }
    public required TimeSpan TotalTime { get; init; }
    public required decimal TotalWeightVolume { get; init; }
    public required string TotalWeightVolumeWithMeasure { get; init; }

    public required ImmutableArray<WorkoutExerciseStatistics> Exercises { get; init; }
}

public record WorkoutExerciseStatistics
{
    public required string ExerciseName { get; set; }
    public required ExerciseSetMetrics? MaxSet { get; set; }
}