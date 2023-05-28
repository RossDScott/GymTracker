using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

[FeatureState]
public record ExercisesState
{
    public ImmutableArray<Exercise> OriginalList { get; init; } = ImmutableArray<Exercise>.Empty;
    public ImmutableArray<ListItem> Exercises { get; init; } = ImmutableArray<ListItem>.Empty;
    public DetailItem? SelectedExercise { get; init; } = null;
    public ExercisesFilter Filter { get; init; } = new();

    public ImmutableArray<ListItem> DisplayList => Filter.FilterOptions.HasFilter
        ? Filter.Results
        : Exercises;
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

public record ExercisesFilter
{
    public ImmutableArray<string> AvailableBodyTargets { get; init; } = ImmutableArray<string>.Empty;

    public ExercisesFilterOptions FilterOptions { get; init; } = new();

    public ImmutableArray<ListItem> Results { get; init; } = ImmutableArray<ListItem>.Empty;
}

public record ExercisesFilterOptions
{
    public string SearchTerm { get; init; } = string.Empty;
    public ImmutableList<string> BodyTargets { get; init; } = ImmutableList<string>.Empty;
    public bool HasFilter =>
        !string.IsNullOrEmpty(SearchTerm) ||
        BodyTargets.Count > 0;
}