using Fluxor;
using GymTracker.Repository;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Workout.Store;

public class WorkoutEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;

    public WorkoutEffects(IClientStorage clientStorage, NavigationManager navigationManager)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
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

    [EffectMethod]
    public async Task OnStartWorkout(StartWorkoutAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.workoutPlanId);
        var workout = new Models.Workout(workoutPlan);
        workout.WorkoutStart = DateTimeOffset.Now;

        await _clientStorage.CurrentWorkout.SetAsync(workout);

        _navigationManager.NavigateTo($"/workout/perform");
    }
}
