using Fluxor;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.BlazorClient.Features.Exercises.Components;
using GymTracker.BlazorClient.Features.Exercises.Store;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Extensions;
using GymTracker.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public class WorkoutPlansEffects
{
    private readonly IClientStorage _clientStorage;

    public WorkoutPlansEffects(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchWorkoutPlansInitialStateAction(FetchWorkoutPlansInitialStateAction action, IDispatcher dispatcher)
    {
        await LoadWorkoutPlans(dispatcher);

        dispatcher.Dispatch(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("WorkoutPlans", "/workout-plans", false, Icons.Material.Filled.List)
        }));
    }

    [EffectMethod]
    public async Task OnFetchWorkoutPlan(FetchWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var workoutPlan =await _clientStorage.WorkoutPlans.FindByIdAsync(action.Id);

        await dispatcher.DispatchWithDelay(new SetWorkoutPlanAction(workoutPlan));
        await dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("WorkoutPlans", "/workout-plans", false, Icons.Material.Filled.List),
            new BreadcrumbItem(workoutPlan.Name, $"/workout-plans", false, Icons.Material.Filled.Edit),
        }));
    }

    [EffectMethod]
    public async Task OnAddExerciseToWorkoutPlan(AddExerciseToWorkoutPlan action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = await _clientStorage.Exercises.FindByIdAsync(action.ExerciseId);
        var order = workoutPlan.PlannedExercises.Count();
   
        var plannedExercise = new PlannedExercise(exercise) 
        { 
            Order = order,
            RestInterval = TimeSpan.FromSeconds(150)
        };

        var plannedSets = order == 0
                ? new List<PlannedExerciseSet>
                    {
                        new PlannedExerciseSet{ Order = 0, OrderForSetType = 1, SetType = "Warm-up",
                            TargetMetrics = new ExerciseSetMetrics{ Reps = 10 }},
                        new PlannedExerciseSet{Order = 1, OrderForSetType = 2, SetType = "Warm-up",
                            TargetMetrics = new ExerciseSetMetrics{ Reps = 10 }}
                    }
                : new List<PlannedExerciseSet>();

        var setStartPos = order == 0 ? 2 : 0;
        var defaultSets = new List<PlannedExerciseSet>
        {
            new PlannedExerciseSet{ Order = setStartPos, OrderForSetType = 1, SetType = "Set",
                TargetMetrics = new ExerciseSetMetrics{ Reps = 8 }},
            new PlannedExerciseSet{ Order = setStartPos + 1, OrderForSetType = 2, SetType = "Set",
                TargetMetrics = new ExerciseSetMetrics{ Reps = 8 }},
            new PlannedExerciseSet{ Order = setStartPos + 2, OrderForSetType = 3, SetType = "Set",
                TargetMetrics = new ExerciseSetMetrics{ Reps = 8 }}
        };
        plannedSets.AddRange(defaultSets);
        plannedExercise.PlannedSets = plannedSets;

        workoutPlan.PlannedExercises.Add(plannedExercise);
        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);

        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
    }

    [EffectMethod]
    public async Task OnUpsertWorkoutPlan(UpsertWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.WorkoutPlan;
        
        var workoutPlan = await _clientStorage.WorkoutPlans.FindOrDefaultByIdAsync(updateDTO.Id)
            ?? new WorkoutPlan { Id = updateDTO.Id };

        workoutPlan.Name = updateDTO.Name;
        workoutPlan.IsAcitve = updateDTO.IsActive;

        var response = await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));

        await LoadWorkoutPlans(dispatcher);
    }

    [EffectMethod]
    public async Task OnFetchExercise(FetchExerciseAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = workoutPlan.PlannedExercises.Single(x => x.Id == action.ExerciseId);

        dispatcher.Dispatch(new SetExerciseAction(exercise));
    }

    [EffectMethod]
    public async Task OnChangeOrder(ChangeOrderAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercises = workoutPlan.PlannedExercises;
        var targetExercise = exercises.Single(x => x.Id == action.ExerciseId);

        exercises.ChangeOrder(targetExercise, action.Direction);

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
        await dispatcher.DispatchWithDelay(new SetExerciseAction(targetExercise));
    }

    private async Task LoadWorkoutPlans(IDispatcher dispatcher)
    {
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        dispatcher.Dispatch(new SetWorkoutPlansAction(workoutPlans));
    }
}
