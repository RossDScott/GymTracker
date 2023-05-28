using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

[FeatureState]
public record ExercisesState
{
    public IImmutableList<ListItem> Exercises { get; set; } = ImmutableList<ListItem>.Empty;
    public DetailItem? SelectedExercise { get; set; } = null;

}

public record ListItem(Guid Id, string Name, bool IsAcitve);
public record DetailItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public MetricType MetricType { get; init; }
    public ImmutableArray<string> BodyTarget { get; init; } = ImmutableArray<string>.Empty;
    public bool IsActive { get; set; } = true;
}

public record FilterOptions
{
    public ImmutableArray<string> AvailableBodyTargets { get; set; } = ImmutableArray<string>.Empty;

}