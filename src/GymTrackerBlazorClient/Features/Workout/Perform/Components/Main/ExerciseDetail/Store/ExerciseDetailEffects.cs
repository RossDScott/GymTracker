using Fluxor;
using GymTracker.BlazorClient.Extensions;
using GymTracker.BlazorClient.Features.Workout.Perform.Components.SideBar.Timers.CountdownTimer.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models;
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
    public async Task OnSetSelectedExercise(SetSelectedExerciseAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.ExerciseId);

        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));

        var settings = await _clientStorage.AppSettings.GetAsync();
        dispatcher.DispatchWithDelay(new SetSetTypesAction(settings!.SetType));
    }

    [EffectMethod]
    public async Task OnSetSetData(SetSetDataAction action, IDispatcher dispatcher)
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

        if (!set.Completed && (set.Metrics.Weight.HasValue || set.Metrics.Reps.HasValue))
            dispatcher.DispatchWithDelay(new ToggleSetCompletedAction(exercise.Id, set.Id, false));
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
    public async Task OnToggleSetCompleted(ToggleSetCompletedAction action, IDispatcher dispatcher)
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

            if (action.AutoUnselect)
                dispatcher.DispatchWithDelay(new SetSelectedSetAction(null));

            if (exercise.PlannedExercise?.AutoTriggerRestTimer ?? false)
            {
                var interval = set.SetType switch
                {
                    DefaultData.SetType.WarmUp => TimeSpan.FromMinutes(1),
                    DefaultData.SetType.Set => exercise.PlannedExercise.RestInterval,
                    _ => TimeSpan.FromMinutes(1),
                };
                dispatcher.Dispatch(new CountdownTimerStartWithDurationAction(interval));
            }
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

    [EffectMethod]
    public async Task OnDeleteSet(DeleteSetAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.WorkoutExerciseId);
        var set = exercise.Sets.Single(x => x.Id == action.SetId);

        exercise.Sets.Remove(set);

        await _clientStorage.CurrentWorkout.SetAsync(workout);
        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }

    [EffectMethod]
    public async Task OnAddSet(AddSetAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.WorkoutExerciseId);

        var order = exercise.Sets
                            .OrderByDescending(x => x.Order)
                            .FirstOrDefault()?
                            .Order + 1 ?? 0;


        var previousForSetType = exercise.Sets
                                      .Where(x => x.SetType == action.SetType)
                                      .OrderByDescending(x => x.OrderForSetType)
                                      .FirstOrDefault();

        var orderForSetType = previousForSetType?.OrderForSetType + 1 ?? 0;
        PlannedExerciseSet? plannedExerciseSet = previousForSetType?.PlannedExerciseSet;

        if (plannedExerciseSet != null)
            plannedExerciseSet = plannedExerciseSet with { Id = Guid.NewGuid() };

        var newSet = new WorkoutExerciseSet(null)
        {
            SetType = action.SetType,
            Order = order,
            OrderForSetType = orderForSetType,
            PlannedExerciseSet = plannedExerciseSet
        };

        exercise.Sets.Add(newSet);

        await _clientStorage.CurrentWorkout.SetAsync(workout);
        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));
    }
}
