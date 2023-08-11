using Fluxor;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.BlazorClient.Features.Exercises.Components;
using GymTracker.BlazorClient.Features.Exercises.Store;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Extensions;
using GymTracker.Domain.Models;
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
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();

        dispatcher.Dispatch(new SetWorkoutPlansAction(workoutPlans));

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
            new BreadcrumbItem(workoutPlan.Name, $"/workout-plans/{workoutPlan.Id}", false, Icons.Material.Filled.Edit),
        }));
    }

    [EffectMethod]
    public async Task OnAddExerciseToWorkoutPlan(AddExerciseToWorkoutPlan action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);

    }

    [EffectMethod]
    public async Task OnAddOrUpdateWorkoutPlan(AddOrUpdateWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.WorkoutPlan;
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        var workoutPlan = await _clientStorage.WorkoutPlans.FindOrDefaultByIdAsync(action.WorkoutPlan.Id);

        var isNew = false;
        if (workoutPlan is null)
        {
            workoutPlan = new WorkoutPlan { Id = updateDTO.Id };
            workoutPlans.Add(workoutPlan);
            isNew = true;
        }

        workoutPlan.Name = updateDTO.Name;
        workoutPlan.IsAcitve = updateDTO.IsActive;

        await _clientStorage.WorkoutPlans.SetAsync(workoutPlans);
        dispatcher.Dispatch(new SetWorkoutPlansAction(workoutPlans));
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));

        if (isNew)
            dispatcher.Dispatch(new NavigateToNewWorkoutPlanAction(workoutPlan.Id));
    }
}
