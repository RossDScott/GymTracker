using Fluxor;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Repository;
using Microsoft.AspNetCore.Components;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.Main.ExerciseDetail.Store;

public class ExerciseDetailEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;

    public ExerciseDetailEffects(IClientStorage clientStorage, NavigationManager navigationManager)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task OnSetSelectedExerciseAction(SetSelectedExerciseAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.ExerciseId);

        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }
}
