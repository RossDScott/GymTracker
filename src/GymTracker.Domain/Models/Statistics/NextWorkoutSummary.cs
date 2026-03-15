using System.Collections.Immutable;

namespace GymTracker.Domain.Models.Statistics;

public record NextWorkoutSummary
{
    public Guid WorkoutPlanId { get; init; }
    public string WorkoutPlanName { get; init; } = string.Empty;
    public DateTimeOffset? LastCompletedOn { get; init; }
    public ImmutableArray<PlannedExercise> Exercises { get; init; }
        = ImmutableArray<PlannedExercise>.Empty;
}
