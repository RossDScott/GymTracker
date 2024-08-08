using Fluxor;
using GymTracker.BlazorClient.Features.Workout.End.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;

namespace GymTracker.BlazorClient.Features.Home.Store;

[FeatureState]
public record HomeState
{
    public bool HasExistingWorkout { get; init; } = false;
}

public record InitaliseHomeAction();
public record SetHasExistingWorkoutAction(bool HasExistingWorkout);
public record SetWorkoutHistoryAction(IEnumerable<WorkoutPlanStatistics> Statistics);

public class HomeEffects
{
    private readonly IClientStorage _clientStorage;

    public HomeEffects(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnInitaliseHome(InitaliseHomeAction action, IDispatcher dispatcher)
    {
        var hasExistingWorkout = await _clientStorage.CurrentWorkout.GetAsync() is not null;
        dispatcher.Dispatch(new SetHasExistingWorkoutAction(hasExistingWorkout));
    }

    [EffectMethod]
    public Task OnSetWorkout(SetWorkoutAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new SetHasExistingWorkoutAction(true));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task OnConfirmEndWorkout(ConfirmEndWorkoutAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new SetHasExistingWorkoutAction(false));
        return Task.CompletedTask;
    }

    public static class HomeReducer
    {
        [ReducerMethod]
        public static HomeState OnSetHasExistingWorkout(HomeState state, SetHasExistingWorkoutAction action)
            => state with { HasExistingWorkout = action.HasExistingWorkout };
    }
}