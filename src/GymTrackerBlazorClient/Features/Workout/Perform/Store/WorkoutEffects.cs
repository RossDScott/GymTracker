using Fluxor;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

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
    public async Task OnStartWorkout(StartWorkoutAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.workoutPlanId);
        var workout = new Models.Workout(workoutPlan);
        workout.WorkoutStart = DateTimeOffset.Now;

        await _clientStorage.CurrentWorkout.SetAsync(workout);

        dispatcher.Dispatch(new SetWorkoutAction(workout));

        _navigationManager.NavigateTo($"/workout/perform");
    }

    [EffectMethod]
    public async Task OnContinueWorkout(ContinueWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));

        dispatcher.Dispatch(new SetWorkoutAction(workout));

        _navigationManager.NavigateTo("workout/perform");
    }
}
