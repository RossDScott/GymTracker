using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

[FeatureState]
public record WorkoutState
{
    public WorkoutDetail Workout { get; init; } = default!;


}

public record WorkoutDetail
{
    public Guid Id { get; init; }

    public DateTimeOffset WorkoutStart { get; init; } = DateTimeOffset.Now;
    public DateTimeOffset? WorkoutEnd { get; init; }
}
