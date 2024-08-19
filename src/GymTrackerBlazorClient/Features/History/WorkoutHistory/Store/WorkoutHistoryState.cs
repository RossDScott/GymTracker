using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

[FeatureState]
public record WorkoutHistoryState
{
    public bool Initalised { get; init; } = false;
    public Guid SelectedWorkoutPlanId { get; init; }
    public ImmutableArray<ListItem> WorkoutPlans { get; init; } = ImmutableArray<ListItem>.Empty;
    public ImmutableArray<DateOnly> Dates { get; init; } = ImmutableArray<DateOnly>.Empty;
    public ImmutableArray<Exercise> Exercises { get; init; } = ImmutableArray<Exercise>.Empty;
}

public record Exercise
{
    public required string ExerciseName { get; init; }
    public required ImmutableArray<string> SetNames { get; init; }
    public required ImmutableArray<ExerciseRecord> Records { get; set; }
}

public record ExerciseRecord
{
    public required DateOnly Date { get; init; }
    public required ImmutableArray<Set> Sets { get; init; }
    public required string TotalVolume { get; set; }
}

public record Set
{
    public required string SetName { get; init; }
    public required string Measure { get; init; }
}

