using Fluxor;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

public class EndWorkoutEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IState<EndWorkoutState> _state;

    public EndWorkoutEffects(
        IClientStorage clientStorage,
        NavigationManager navigationManager,
        IState<EndWorkoutState> state)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
        _state = state;
    }

    [EffectMethod]
    public async Task OnEndWorkout(EndWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));
        var workoutStatistics = await _clientStorage.WorkoutPlanStatistics.FindOrDefaultByIdAsync(workout.Plan.Id);

        dispatcher.Dispatch(new SetEndWorkoutAction(workout, workoutStatistics));
        dispatcher.Dispatch(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Workout", "/workout/end", false, Icons.Material.Filled.SportsMartialArts),
            new BreadcrumbItem("End", "/workout/end", false, Icons.Material.Filled.Close)
        }));

        _navigationManager.NavigateTo("/workout/end");
    }

    [EffectMethod]
    public Task OnCancelEndWorkout(CancelEndWorkoutAction action, IDispatcher dispatcher)
    {
        _navigationManager.NavigateTo("/workout/perform");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task OnAbandonWorkout(AbandonWorkoutAction action, IDispatcher dispatcher)
    {
        await _clientStorage.CurrentWorkout.DeleteAsync();
        _navigationManager.NavigateTo("/");
    }

    [EffectMethod]
    public async Task OnConfirmEndWorkout(ConfirmEndWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));
        var state = _state.Value;
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(workout.Plan.Id);

        workout.WorkoutStart = state.WorkoutStartDate!.Value.Add(state.WorkoutStartTime!.Value);
        workout.WorkoutEnd = state.WorkoutEndDate!.Value.Add(state.WorkoutEndTime!.Value);

        foreach (var exercise in state.ExerciseList)
        {
            var workoutPlanExercise = workoutPlan.PlannedExercises.SingleOrDefault(x => x.Id == exercise.PlannedWorkoutExerciseId);

            if (workoutPlanExercise == null) continue;

            var acceptedProgress = exercise.ProgressSets.Single(x => x.Selected);
            var sets = workoutPlanExercise.PlannedSets.Where(x => x.SetType == DefaultData.SetType.Set);
            foreach (var item in sets.Select((set, i) => new { set, i }))
            {
                if (exercise.MetricType != MetricType.Reps || item.i == sets.Count() - 1)
                    item.set.TargetMetrics = acceptedProgress.Metrics;
            }

            var workoutExercise = workout.Exercises.Single(x => x.Id == exercise.WorkoutExerciseId);
            foreach (var set in workoutExercise.Sets)
            {
                if (set.CompletedOn > workout.WorkoutEnd)
                    set.CompletedOn = workout.WorkoutEnd;

                if (set.CompletedOn < workout.WorkoutStart)
                    set.CompletedOn = workout.WorkoutStart;
            }
        }

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        await _clientStorage.Workouts.UpsertAsync(workout);
        await _clientStorage.CurrentWorkout.DeleteAsync();

        _navigationManager.NavigateTo("/");
    }
}
