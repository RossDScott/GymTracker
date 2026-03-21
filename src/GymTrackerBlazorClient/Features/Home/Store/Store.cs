using System.Text.Json;
using ApexCharts;
using Fluxor;
using GymTracker.BlazorClient.Features.Workout.End.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;
using GymTracker.LocalStorage.IndexedDb;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Home.Store;

[FeatureState]
public record HomeState
{
    public bool HasExistingWorkout { get; init; } = false;
    public ImmutableArray<WorkoutStatistic> CompletedWorkouts { get; init; } = ImmutableArray<WorkoutStatistic>.Empty;
    public ImmutableArray<ChartState> Charts { get; init; } = ImmutableArray<ChartState>.Empty;
}

public record ChartState
{
    public required ExerciseStatistic ExerciseStatistic { get; init; }
    public ApexChartOptions<ExerciseLog> ChartOptions
    => new()
    {
        Chart = new Chart
        {
            Background = "#32333d",
            Toolbar = new Toolbar
            {
                Show = false
            },
            ForeColor = "#fff",
            Width = "100%",
            Height = "200px"
        },
        Colors = new List<string> { "#77B6EA" },
    };
}

public record InitaliseHomeAction();
public record SetHasExistingWorkoutAction(bool HasExistingWorkout);
public record SetWorkoutStatisticsDataAction(IEnumerable<WorkoutStatistic> Statistics);
public record SetExerciseStatisticsDataAction(IEnumerable<ExerciseStatistic> Statistics);

public class HomeEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly IIndexedDbService _db;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HomeEffects(IClientStorage clientStorage, IIndexedDbService db, IDispatcher dispatcher)
    {
        _clientStorage = clientStorage;
        _db = db;

        clientStorage.WorkoutStatistics.SubscribeToChanges((stats) =>
        {
            dispatcher.Dispatch(new SetWorkoutStatisticsDataAction(stats));
            return Task.CompletedTask;
        });

        clientStorage.ExerciseStatistics.SubscribeToChanges((stats) =>
        {
            dispatcher.Dispatch(new SetExerciseStatisticsDataAction(stats));
            return Task.CompletedTask;
        });
    }

    [EffectMethod]
    public async Task OnInitaliseHome(InitaliseHomeAction action, IDispatcher dispatcher)
    {
        var results = await _db.GetBatchAsync([
            new { storeName = "CurrentWorkout", key = (object)"singleton" },
            new { storeName = "WorkoutStatistics" },
            new { storeName = "ExerciseStatistics" }
        ]);

        var currentWorkout = results[0].ValueKind != JsonValueKind.Null
            ? results[0].Deserialize<SingletonWrapper<GymTracker.Domain.Models.Workout>>(_jsonOptions)?.Value
            : null;
        var workoutStats = results[1].ValueKind != JsonValueKind.Null
            ? results[1].Deserialize<List<WorkoutStatistic>>(_jsonOptions)
            : null;
        var exerciseStats = results[2].ValueKind != JsonValueKind.Null
            ? results[2].Deserialize<List<ExerciseStatistic>>(_jsonOptions)
            : null;

        dispatcher.Dispatch(new SetHasExistingWorkoutAction(currentWorkout is not null));

        if (workoutStats != null)
            dispatcher.Dispatch(new SetWorkoutStatisticsDataAction(workoutStats));

        if (exerciseStats != null)
            dispatcher.Dispatch(new SetExerciseStatisticsDataAction(exerciseStats));
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
        public static HomeState OnSetCompletedWorkouts(HomeState state, SetWorkoutStatisticsDataAction action)
            => state with
            {
                CompletedWorkouts = action.Statistics
                                            .OrderByDescending(x => x.CompletedOn)
                                            .Take(20)
                                            .ToImmutableArray()
            };

        [ReducerMethod]
        public static HomeState OnSetExerciseStatisticsData(HomeState state, SetExerciseStatisticsDataAction action)
            => state with
            {
                Charts = action.Statistics
                    .Where(x => x.ShowChartOnHomePage)
                    .Select(x => new ChartState { ExerciseStatistic = x })
                    .ToImmutableArray()
            };
    }
}