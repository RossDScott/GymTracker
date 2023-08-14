using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

[FeatureState]
public record WorkoutPlansState
{
    public ImmutableArray<Models.WorkoutPlan> OriginialList { get; init; } = ImmutableArray<Models.WorkoutPlan>.Empty;
    public ImmutableArray<ListItem> WorkoutPlans { get; init; } = ImmutableArray<ListItem>.Empty;
    public WorkoutPlanDetail? SelectedWorkoutPlan { get; init; } = null;
    public PlannedExerciseDetail? SelectedExercise { get; set; } = null;
    public PlannedSetDetail? SelectedSet { get; set; }
}

public record WorkoutPlanDetail
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;

    public ImmutableArray<ListItem> PlannedExercises { get; init; } = ImmutableArray<ListItem>.Empty;

    public bool IsActive { get; init; } = true;
}

public record PlannedExerciseDetail
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;

    public ImmutableArray<PlannedSetDetail> PlannedSets { get; init; } = ImmutableArray<PlannedSetDetail>.Empty;

    public TimeSpan RestInterval { get; init; }
    public bool AutoTriggerRestTimer { get; init; } = true;
}

public record PlannedSetDetail
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Order { get; set; }
    public string SetType { get; set; } = default!;
    public int OrderForSetType { get; set; }
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Time { get; set; }
    public decimal? Distance { get; set; }
}