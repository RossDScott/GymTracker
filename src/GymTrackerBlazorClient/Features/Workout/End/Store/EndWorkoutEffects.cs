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
        var workoutStatistics = await _clientStorage.WorkoutPlanStatistics.FindOrDefaultByIdAsync(workout.Plan.Id);

        _dispatcher.Dispatch(new SetEndWorkoutAction(workout, workoutStatistics));
        _dispatcher.Dispatch(new SetBreadcrumbAction(new[]
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
    public async Task OnConfirmEndWorkout(ConfirmEndWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));
        var state = _state.Value;
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(workout.Plan.Id);

        workout.WorkoutEnd = _state.Value.WorkoutEnd;
        foreach (var exercise in state.ExerciseList)
        {
            var workoutPlanExercise = workoutPlan.PlannedExercises.SingleOrDefault(x => x.Id == exercise.PlannedWorkoutExerciseId);

            if (workoutPlanExercise == null) continue;

            var acceptedProgress = exercise.ProgressSets.Single(x => x.Selected);
            var sets = workoutPlanExercise.PlannedSets.Where(x => x.SetType == DefaultData.SetType.Set);
            foreach (var set in sets)
            {
                set.TargetMetrics = acceptedProgress.Metrics;
            }
        }

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        await _clientStorage.Workouts.UpsertAsync(workout);
        await _clientStorage.CurrentWorkout.DeleteAsync();

        _navigationManager.NavigateTo($"/");
    }
}
