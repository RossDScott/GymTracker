using System.Collections.Immutable;

namespace GymTracker.Domain.Models.Statistics;
public record WorkoutPlanStatistic
{
    public required Guid WorkoutPlanId { get; init; }

    public required WorkoutStatistic PreviousWorkout { get; init; }

    public decimal BestWeightTotalVolumeIn6Months { get; init; }
}

public record WorkoutStatistic
{
    public required Guid WorkoutId { get; init; }
    public required Guid WorkoutPlanId { get; init; }
    public required string WorkoutPlanName { get; init; }
    public required DateTimeOffset CompletedOn { get; init; }
    public required TimeSpan TotalTime { get; init; }
    public required decimal TotalWeightVolume { get; init; }
    public required string TotalWeightVolumeWithMeasure { get; init; }
    public int TotalReps { get; init; } = 0;
    public bool IsRepsOnly { get; set; } = false;

    public required ImmutableArray<WorkoutExerciseStatistics> Exercises { get; init; }
}

public record WorkoutExerciseStatistics
{
    public required string ExerciseName { get; set; }
    public required ExerciseSetMetrics? MaxSet { get; set; }
    public MetricType MetricType { get; set; }
    public required bool AllCompleted { get; set; }
    public required bool AnyCompleted { get; set; }
}