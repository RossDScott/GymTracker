using Fluxor;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

public class EndWorkoutEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IState<EndWorkoutState> _state;
    private readonly IBackupOrchestrator _backupOrchestrator;

    public EndWorkoutEffects(
        IClientStorage clientStorage,
        NavigationManager navigationManager,
        IState<EndWorkoutState> state,
        IBackupOrchestrator backupOrchestrator)
    {
        _clientStorage = clientStorage;
        _navigationManager = navigationManager;
        _state = state;
        _backupOrchestrator = backupOrchestrator;
    }

    [EffectMethod]
    public async Task OnEndWorkout(EndWorkoutAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));
        var workoutStatistics = await _clientStorage.WorkoutPlanStatistics.FindOrDefaultByIdAsync(workout.Plan.Id);

        var milestonesList = new List<ExerciseMilestones>();
        foreach (var workoutExercise in workout.Exercises)
        {
            var completedSets = workoutExercise.Sets
                .Where(s => s.SetType == DefaultData.SetType.Set && s.Completed)
                .Select(s => s.Metrics)
                .ToList();

            if (!completedSets.Any()) continue;

            var exerciseStat = await _clientStorage.ExerciseStatistics
                .FindOrDefaultByIdAsync(workoutExercise.Exercise.Id);

            var totalVolume = workoutExercise.Sets
                .Where(s => s.Completed)
                .Select(s => s.Metrics)
                .GetTotalVolume(workoutExercise.Exercise.MetricType);

            var exerciseMilestones = MilestoneDetector.DetectExerciseMilestones(
                workoutExercise.Exercise.Id,
                workoutExercise.Exercise.Name,
                workoutExercise.Exercise.MetricType,
                completedSets,
                totalVolume,
                exerciseStat);

            milestonesList.Add(exerciseMilestones);
        }

        dispatcher.Dispatch(new SetEndWorkoutAction(workout, workoutStatistics, milestonesList.ToImmutableArray()));
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
        await _backupOrchestrator.DeleteBlobAsync(_clientStorage.CurrentWorkout.KeyName);
        _navigationManager.NavigateTo("/");
    }

    [EffectMethod]
    public async Task OnConfirmEndWorkout(ConfirmEndWorkoutAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new SetIsSavingAction(true));

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
            var sets = workoutPlanExercise.PlannedSets.Where(x => x.SetType == DefaultData.SetType.Set).ToList();
            foreach (var item in sets.Select((set, i) => new { set, i }))
            {
                if (exercise.MetricType != MetricType.Reps || item.i == sets.Count() - 1)
                    item.set.TargetMetrics = acceptedProgress.Metrics with { };
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
        await _backupOrchestrator.DeleteBlobAsync(_clientStorage.CurrentWorkout.KeyName);

        dispatcher.Dispatch(new SetIsSavingAction(false));

        _navigationManager.NavigateTo("/");
    }
}
