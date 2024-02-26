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

    [EffectMethod]
    public async Task OnSetSetDataAction(SetSetDataAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.WorkoutExerciseId);
        var set = exercise.Sets.Single(x => x.Id == action.EditSet.Id);

        if (set.PlannedExerciseSet != null)
        {
            set.PlannedExerciseSet.TargetMetrics.Weight = action.EditSet.TargetWeight;
            set.PlannedExerciseSet.TargetMetrics.Reps = action.EditSet.TargetReps;
            set.Metrics.Weight = action.EditSet.ActualWeight;
            set.Metrics.Reps = action.EditSet.ActualReps;
            set.Completed = true;
        }

        await _clientStorage.CurrentWorkout.SetAsync(workout);
        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }
}
