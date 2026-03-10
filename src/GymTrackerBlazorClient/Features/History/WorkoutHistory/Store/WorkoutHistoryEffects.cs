using Fluxor;
using GymTracker.BlazorClient.Extensions;
using GymTracker.BlazorClient.Features.Workout.End.Store;
using GymTracker.BlazorClient.Shared;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

public class WorkoutHistoryEffects : EffectsBase
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IState<WorkoutHistoryState> _state;

    public WorkoutHistoryEffects(
            IClientStorage clientStorage,
            NavigationManager navigationManager,
            IState<WorkoutHistoryState> state,
            ErrorService errorService)
        : base(errorService)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
        _state = state;
    }

    [EffectMethod]
    public async Task OnInitialise(InitialiseAction action, IDispatcher dispatcher) =>
        await SafeHandle(async () =>
        {
            var plansTask = _clientStorage.WorkoutPlans.GetOrDefaultAsync();
            var workoutsTask = _clientStorage.Workouts.GetOrDefaultAsync();
            await Task.WhenAll(plansTask.AsTask(), workoutsTask.AsTask());

            dispatcher.Dispatch(new SetInitialDataAction(plansTask.Result, workoutsTask.Result));
        });

    [EffectMethod]
    public async Task OnViewWorkoutHistory(ViewWorkoutHistoryAction action, IDispatcher dispatcher) =>
        await SafeHandle(() =>
        {
            if (!_state.Value.Initalised)
                dispatcher.DispatchWithDelay(new InitialiseAction());

            _navigationManager.NavigateTo("history/workouthistory");
            dispatcher.DispatchWithDelay(new SetWorkoutPlanIdAction(action.WorkoutPlanId), 2);

            return Task.CompletedTask;
        });

    [EffectMethod]
    public async Task OnConfirmEndWorkout(ConfirmEndWorkoutAction action, IDispatcher dispatcher) =>
        await SafeHandle(() =>
        {
            if (_state.Value.Initalised)
                dispatcher.DispatchWithDelay(new InitialiseAction());

            return Task.CompletedTask;
        });
}
