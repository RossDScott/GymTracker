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
        
        var workoutPlan = await _clientStorage.WorkoutPlans.FindOrDefaultByIdAsync(updateDTO.Id)
            ?? new WorkoutPlan { Id = updateDTO.Id };

        workoutPlan.Name = updateDTO.Name;
        workoutPlan.IsAcitve = updateDTO.IsActive;

        var response = await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));

        await LoadWorkoutPlans(dispatcher);

        if (response == UpsertResponse.New)
            dispatcher.Dispatch(new NavigateToNewWorkoutPlanAction(workoutPlan.Id));
    }

    private async Task LoadWorkoutPlans(IDispatcher dispatcher)
    {
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        dispatcher.Dispatch(new SetWorkoutPlansAction(workoutPlans));
    }
}
