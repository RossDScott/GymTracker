using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.ExercisePicker.Store;

[FeatureState]
public record ExercisesState
{
    public ImmutableArray<string> TargetBodyParts { get; set; } = ImmutableArray<string>.Empty;
    public ImmutableArray<string> Equipment { get; set; } = ImmutableArray<string>.Empty;

    public ImmutableArray<Models.Exercise> OriginalList { get; init; } = ImmutableArray<Models.Exercise>.Empty;
    public ImmutableArray<ListItem> Exercises { get; init; } = ImmutableArray<ListItem>.Empty;
    public DetailItem? SelectedExercise { get; init; } = null;
    public ExercisesFilter Filter { get; init; } = new();
}

public record DetailItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public Models.MetricType MetricType { get; init; }
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
}

public record CheckItem(string Name, bool IsChecked);