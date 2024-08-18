using Fluxor;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

public class WorkoutHistoryEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;

    public WorkoutHistoryEffects(IClientStorage clientStorage, NavigationManager navigationManager)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task OnViewWorkoutHistory(ViewWorkoutHistoryAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.Workouts.FindByIdAsync(action.WorkoutId);
        dispatcher.Dispatch(new SetWorkoutPlanIdAction(workout.Plan.Id));
        _navigationManager.NavigateTo("history/workouthistory");
    }

    [EffectMethod]
    public async Task OnSetWorkoutPlanId(SetWorkoutPlanIdAction action, IDispatcher dispatcher)
    {
        var workouts = await _clientStorage.Workouts.GetOrDefaultAsync();
        var workoutsForPlan = workouts.Where(x => x.Plan.Id == action.Id).ToList();

        dispatcher.Dispatch(new SetWorkoutHistoryAction(workoutsForPlan));
    }
}
