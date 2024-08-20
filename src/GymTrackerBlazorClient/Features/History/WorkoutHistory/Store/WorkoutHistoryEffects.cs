using Fluxor;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

public class WorkoutHistoryEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IState<WorkoutHistoryState> _state;

    public WorkoutHistoryEffects(
            IClientStorage clientStorage,
            NavigationManager navigationManager,
            IState<WorkoutHistoryState> state)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
        _state = state;
    }

    private async Task Initialise(IDispatcher dispatcher)
    {
        var plans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        var workouts = await _clientStorage.Workouts.GetOrDefaultAsync();

        dispatcher.Dispatch(new SetInitialDataAction(plans, workouts));
    }

    [EffectMethod]
    public async Task OnViewWorkoutHistory(ViewWorkoutHistoryAction action, IDispatcher dispatcher)
    {
        if (!_state.Value.Initalised)
            await Initialise(dispatcher);

        var workout = await _clientStorage.Workouts.FindByIdAsync(action.WorkoutId);
        dispatcher.Dispatch(new SetWorkoutPlanIdAction(workout.Plan.Id));
        _navigationManager.NavigateTo("history/workouthistory");
    }
}
