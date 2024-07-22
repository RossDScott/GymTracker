using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

[FeatureState]
public record EndWorkoutState
{
    public DateTimeOffset WorkoutEnd { get; init; }
    public TimeSpan Duration { get; set; }
    public ImmutableArray<ExerciseDetail> ExerciseList { get; init; } = ImmutableArray<ExerciseDetail>.Empty;
}

public record ExerciseDetail
{
    public Guid WorkoutExerciseId { get; set; }
    public required string ExerciseName { get; set; }
    public MetricType MetricType { get; init; }
    public ImmutableArray<ProgressSet> ProgressSets { get; init; } = ImmutableArray<ProgressSet>.Empty;
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