using Fluxor;
using GymTracker.BlazorClient.Extensions;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.LocalStorage.Core;
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
        }

        set.Metrics.Weight = action.EditSet.ActualWeight;
        set.Metrics.Reps = action.EditSet.ActualReps;

        await _clientStorage.CurrentWorkout.SetAsync(workout);
        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }

    [EffectMethod]
    public async Task OnSetWeightIncrement(SetWeightIncrementAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.WorkoutExerciseId);

        if (exercise.PlannedExercise != null)
            exercise.PlannedExercise.TargetWeightIncrement = action.WeightIncrement;

        await _clientStorage.CurrentWorkout.SetAsync(workout);
        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }

    [EffectMethod]
    public async Task OnToggleSetCompletedAction(ToggleSetCompletedAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.WorkoutExerciseId);
        var set = exercise.Sets.Single(x => x.Id == action.SetId);

        set.Completed = !set.Completed;
        if (set.Completed)
        {
            set.Metrics.Distance = set.Metrics.Distance ?? set.PlannedExerciseSet?.TargetMetrics.Distance;
            set.Metrics.Reps = set.Metrics.Reps ?? set.PlannedExerciseSet?.TargetMetrics.Reps;
            set.Metrics.Weight = set.Metrics.Weight ?? set.PlannedExerciseSet?.TargetMetrics.Weight;
            set.Metrics.Time = set.Metrics.Time ?? set.PlannedExerciseSet?.TargetMetrics.Time;
            set.CompletedOn = DateTimeOffset.Now;

            dispatcher.DispatchWithDelay(new SetSelectedSetAction(null));
        }
        else
        {
            set.Metrics.Distance = null;
            set.Metrics.Reps = null;
            set.Metrics.Weight = null;
            set.Metrics.Time = null;
            set.CompletedOn = null;
        }

        await _clientStorage.CurrentWorkout.SetAsync(workout);
        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }
}
