using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

[FeatureState]
public record ExercisesState
{
    public ImmutableArray<string> TargetBodyParts { get; set; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> Equipment { get; set; } = ImmutableArray<string>.Empty;

    public ImmutableArray<Exercise> OriginalList { get; init; } = ImmutableArray<Exercise>.Empty;
    public ImmutableArray<ListItem> Exercises { get; init; } = ImmutableArray<ListItem>.Empty;
    public DetailItem? SelectedExercise { get; init; } = null;
    public ExercisesFilter Filter { get; init; } = new();

    public ImmutableArray<ListItem> DisplayList => Filter.HasFilter
        ? Filter.Results
        : Exercises;
}


public record DetailItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public MetricType MetricType { get; init; }
    public ImmutableArray<string> BodyTarget { get; init; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> Equipment { get; init; } = ImmutableArray<string>.Empty;
    public bool IsActive { get; set; } = true;
}

public enum ActiveFilterOption
{
    Active,
    Inactive,
    NotSet
}

public record ExercisesFilter
{
    public ActiveFilterOption ActiveOption { get; set; } = ActiveFilterOption.Active;
    public string SearchTerm { get; init; } = string.Empty;
    public ImmutableList<CheckItem> BodyTargets { get; init; } = ImmutableList<CheckItem>.Empty;
    public ImmutableList<CheckItem> CheckedBodyTargets => BodyTargets.Where(x => x.IsChecked).ToImmutableList();
    public bool HasFilter =>
        ActiveOption != ActiveFilterOption.NotSet ||
        !string.IsNullOrEmpty(SearchTerm) ||
        BodyTargets.Count(x => x.IsChecked) > 0;

    public ImmutableArray<ListItem> Results { get; init; } = ImmutableArray<ListItem>.Empty;
}

public record CheckItem(string Name, bool IsChecked);