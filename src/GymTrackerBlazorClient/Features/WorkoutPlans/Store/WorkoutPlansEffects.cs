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
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        var workoutPlan = workoutPlans.Single(x => x.Id == action.Id);

        await dispatcher.DispatchWithDelay(new SetWorkoutPlanAction(workoutPlan));
        await dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("WorkoutPlans", "/workout-plans", false, Icons.Material.Filled.List),
            new BreadcrumbItem(workoutPlan.Name, $"/workout-plans/{workoutPlan.Id}", false, Icons.Material.Filled.Edit),
        }));
    }

    [EffectMethod]
    public async Task OnAddOrUpdateWorkoutPlan(AddOrUpdateWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.WorkoutPlan;
        var plans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        var plan = plans.SingleOrDefault(x => x.Id == updateDTO.Id);

        var isNew = false;
        if (plan is null)
        {
            plan = new WorkoutPlan { Id = updateDTO.Id };
            plans.Add(plan);
            isNew = true;
        }

        plan.Name = updateDTO.Name;
        plan.IsAcitve = updateDTO.IsActive;

        await _clientStorage.WorkoutPlans.SetAsync(plans);
        dispatcher.Dispatch(new SetWorkoutPlansAction(plans));
        dispatcher.Dispatch(new SetWorkoutPlanAction(plan));

        if (isNew)
            dispatcher.Dispatch(new NavigateToNewWorkoutPlanAction(plan.Id));
    }
}
