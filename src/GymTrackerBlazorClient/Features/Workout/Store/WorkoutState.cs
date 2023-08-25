using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Store;

[FeatureState]
public record WorkoutState
{
    public WorkoutListOrder WorkoutPlanOrder { get; init; } = WorkoutListOrder.Recent;
    public ImmutableArray<WorkoutPlanListItem> WorkoutPlans { get; init; } = ImmutableArray<WorkoutPlanListItem>.Empty;

    public ImmutableArray<ListItem> WorkoutListOrderOptions = Enum.GetNames(typeof(WorkoutListOrder))
        .Select(name => new ListItem(Guid.NewGuid(), name, true ))
        .ToImmutableArray();

    public ListItem SelectedWorkoutPlanListOrder
        => WorkoutListOrderOptions.Single(x => x.Name == WorkoutPlanOrder.ToString());
}

public enum WorkoutListOrder
{
    Recent,
    All
}

public record WorkoutPlanListItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime? LastPerformedOn { get; set; }
    public ImmutableArray<string> PlannedExercises { get; init; } = ImmutableArray<string>.Empty;
}