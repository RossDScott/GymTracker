using Fluxor;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

public class EndWorkoutEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IState<EndWorkoutState> _state;
    private readonly IDispatcher _dispatcher;

    public EndWorkoutEffects(
        IClientStorage clientStorage,
        NavigationManager navigationManager,
        IState<EndWorkoutState> state,
        IDispatcher dispatcher)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
        _state = state;
        _dispatcher = dispatcher;
    }

    [EffectMethod]
    public async Task OnEndWorkout(EndWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));

        _dispatcher.Dispatch(new SetEndWorkoutAction(workout));
        _dispatcher.Dispatch(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Workout", "/workout/end", false, Icons.Material.Filled.SportsMartialArts),
            new BreadcrumbItem("End", "/workout/end", false, Icons.Material.Filled.Close)
        }));

        _navigationManager.NavigateTo("/workout/end");
    }

    [EffectMethod]
    public async Task OnConfirmEndWorkout(ConfirmEndWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));

        workout.WorkoutEnd = _state.Value.WorkoutEnd;

        await _clientStorage.Workouts.UpsertAsync(workout);
        await _clientStorage.CurrentWorkout.DeleteAsync();

        _navigationManager.NavigateTo($"/");
    }
}
