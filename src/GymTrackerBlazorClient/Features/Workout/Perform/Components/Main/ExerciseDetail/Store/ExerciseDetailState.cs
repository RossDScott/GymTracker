using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.Main.ExerciseDetail.Store;

[FeatureState]
public record ExerciseDetailState
{
    public Guid WorkoutExerciseId { get; set; }
    public MetricType MetricType { get; init; }
    public decimal WeightIncrement { get; set; } = 1;
    public Guid? SelectedSetId { get; init; }

    public ImmutableList<Set> Sets { get; init; } = ImmutableList<Set>.Empty;

    public ImmutableArray<string> SetTypes { get; set; }
}

public record Set
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public int? TargetReps { get; init; }
    public decimal? TargetWeight { get; init; }
    public decimal? TargetTime { get; init; }

    public int? ActualReps { get; init; }
    public decimal? ActualWeight { get; init; }
    public decimal? ActualTime { get; init; }

    public bool Completed { get; init; }
}

public record EditSet
{
    public EditSet(Set set)
    {
        this.Id = set.Id;
        this.Name = set.Name;

        this.TargetReps = set.TargetReps;
        this.TargetWeight = set.TargetWeight;
        this.TargetTime = set.TargetTime;

        this.ActualReps = set.ActualReps;
        this.ActualWeight = set.ActualWeight;
        this.ActualTime = set.TargetTime;

        //this.WeightIncrement = set.DefaultWeightIncrement ?? 1;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }

    public int? TargetReps { get; set; }
    public decimal? TargetWeight { get; set; }
    public decimal? TargetTime { get; set; }

    public int? ActualReps { get; set; }
    public decimal? ActualWeight { get; set; }
    public decimal? ActualTime { get; set; }

    //public decimal WeightIncrement { get; set; }
}
