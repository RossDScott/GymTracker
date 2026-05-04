using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Circuit.Store;

[FeatureState]
public record CircuitPerformState
{
    public Guid WorkoutId { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public int TotalRounds { get; init; }
    public TimeSpan RestBetweenRounds { get; init; }
    public int CurrentRound { get; init; } = 1;
    public int CurrentExerciseIndex { get; init; } = 0;
    public CircuitPhase Phase { get; init; } = CircuitPhase.Exercising;
    public ImmutableArray<CircuitExerciseItem> Exercises { get; init; } = ImmutableArray<CircuitExerciseItem>.Empty;

    public bool IsInitialized => WorkoutId != Guid.Empty;

    public CircuitExerciseItem? CurrentExercise =>
        Exercises.Length > CurrentExerciseIndex ? Exercises[CurrentExerciseIndex] : null;
}

public record CircuitExerciseItem
{
    public Guid WorkoutExerciseId { get; init; }
    public string ExerciseName { get; init; } = string.Empty;
    public MetricType MetricType { get; init; }
    public int? TargetReps { get; init; }
    public decimal? TargetWeight { get; init; }
    public decimal? TargetTime { get; init; }
}

public enum CircuitPhase { Exercising, Resting }
