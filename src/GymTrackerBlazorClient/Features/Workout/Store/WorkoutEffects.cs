using Fluxor;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Store;

public class WorkoutEffects
{
    private readonly IClientStorage _clientStorage;

    public WorkoutEffects(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchWorkoutPlans(FetchWorkoutPlansAction action, IDispatcher dispatcher)
    {
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        var list = workoutPlans
            .Where(x => x.IsAcitve)
            .Select(x => x.ToWorkoutPlanListItem())
            .ToImmutableArray();

        dispatcher.Dispatch(new SetWorkoutPlansAction(list));
    }
}
