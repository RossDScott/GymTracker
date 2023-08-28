using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.PickAPlan;

enum WorkoutListOrder { Recent, All }

public record WorkoutPlanListItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime? LastPerformedOn { get; set; }
    public ImmutableArray<string> PlannedExercises { get; init; } = ImmutableArray<string>.Empty;
}
