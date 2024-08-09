using Fluxor;
using GymTracker.BlazorClient.Features.Workout.End.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Home.Store;

[FeatureState]
public record HomeState
{
    public bool HasExistingWorkout { get; init; } = false;
    public ImmutableArray<WorkoutStatistics> CompletedWorkouts { get; init; } = ImmutableArray<WorkoutStatistics>.Empty;
}

public record InitaliseHomeAction();
public record SetHasExistingWorkoutAction(bool HasExistingWorkout);
public record SetCompletedWorkoutsAction(IEnumerable<WorkoutStatistics> Statistics);

public class HomeEffects
{
    private readonly IClientStorage _clientStorage;

    public HomeEffects(IClientStorage clientStorage, IDispatcher dispatcher)
    {
        _clientStorage = clientStorage;

        clientStorage.WorkoutStatistics.SubscribeToChanges((stats) =>
        {
            dispatcher.Dispatch(new SetCompletedWorkoutsAction(stats));
            return Task.CompletedTask;
        });
    }

    [EffectMethod]
    public async Task OnInitaliseHome(InitaliseHomeAction action, IDispatcher dispatcher)
    {
        var hasExistingWorkout = await _clientStorage.CurrentWorkout.GetAsync() is not null;
        dispatcher.Dispatch(new SetHasExistingWorkoutAction(hasExistingWorkout));

        var statistics = await _clientStorage.WorkoutStatistics.GetAsync();
        if (statistics != null)
            dispatcher.Dispatch(new SetCompletedWorkoutsAction(statistics));
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

        [ReducerMethod]
        public static HomeState OnSetCompletedWorkouts(HomeState state, SetCompletedWorkoutsAction action)
            => state with
            {
                CompletedWorkouts = action.Statistics
                                            .OrderByDescending(x => x.CompletedOn)
                                            .Take(20)
                                            .ToImmutableArray()
            };
    }
}