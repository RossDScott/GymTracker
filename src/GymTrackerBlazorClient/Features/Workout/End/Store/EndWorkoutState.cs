using Fluxor;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Statistics;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

[FeatureState]
public record EndWorkoutState
{
    public required DateTime? WorkoutStartDate { get; set; }
    public required TimeSpan? WorkoutStartTime { get; set; }
    public required DateTime? WorkoutEndDate { get; set; }
    public required TimeSpan? WorkoutEndTime { get; set; }

    public TimeSpan Duration =>
        WorkoutEndDate!.Value.Date.Add(WorkoutEndTime!.Value) -
        WorkoutStartDate!.Value.Date.Add(WorkoutStartTime!.Value);
    public required string TotalVolumeMessage { get; init; }
    public required ImmutableArray<ExerciseDetail> ExerciseList { get; init; }
    public WorkoutPlanStatistic? PreviousStatistics { get; init; } = null;
}

public record ExerciseDetail
{
    public required Guid WorkoutExerciseId { get; init; }
    public Guid? PlannedWorkoutExerciseId { get; init; } = null;
    public required string ExerciseName { get; init; }
    public required MetricType MetricType { get; init; }
    public required ImmutableArray<ProgressSet> ProgressSets { get; init; }
    public required ImmutableArray<ExerciseSetMetrics> CompletedSets { get; set; }
}

public record ProgressSet
{
    public ProgressType ProgressType { get; init; }
    public required ExerciseSetMetrics Metrics { get; init; }
    public bool Selected { get; set; } = false;
}

public enum ProgressType
{
    Previous,
    AutoProgress,
    MaxSet,
    Custom
}