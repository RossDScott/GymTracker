using Fluxor;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.Main.SideBar.Store;

[FeatureState]
public record HistoryState
{
    public int NumberOfMonths { get; set; } = 3;
    public MetricType MetricType { get; set; }
    public ExerciseSetMetrics? MaxSet { get; init; } = null;
    public ExerciseSetMetrics? MaxVolume { get; init; } = null;

    public ImmutableArray<Workout> History { get; init; } = ImmutableArray<Workout>.Empty;
}

public record Workout
{
    public DateTimeOffset CompletedOn { get; init; }
    public required ImmutableArray<WorkoutExerciseSet> Sets { get; init; }
    public required string TotalVolume { get; init; }
}

public record WorkoutExerciseSet
{
    public required string SetDetails { get; init; }
}

public record SetHistoryAction(ExerciseStatistic? ExerciseStatistic, MetricType MetricType);

public static class HistoryStateReducer
{
    [ReducerMethod]
    public static HistoryState OnSetHistory(HistoryState state, SetHistoryAction action)
    {
        if (action.ExerciseStatistic == null)
            return state with { MetricType = action.MetricType };

        var dataForMonths = action.ExerciseStatistic.Logs
            .Where(x => x.WorkoutDateTime > DateTimeOffset.Now.AddMonths(-state.NumberOfMonths))
            .ToList();

        if (!dataForMonths.Any())
            return state with
            {
                MetricType = action.MetricType,
                MaxVolume = null,
                MaxSet = null,
                History = ImmutableArray<Workout>.Empty
            };

        var sets = dataForMonths.SelectMany(x => x.Sets).ToList();

        return state with
        {
            MetricType = action.MetricType,
            MaxSet = sets.GetMaxSet(action.MetricType),
            MaxVolume = sets.GetMaxVolumeSet(action.MetricType),
            History = dataForMonths
            .OrderByDescending(x => x.WorkoutDateTime)
            .Select(x => new Workout
            {
                CompletedOn = x.WorkoutDateTime,
                TotalVolume = x.Sets.ToMeasureTotalVolume(action.MetricType),
                Sets = x.Sets
                        .Select(x => new WorkoutExerciseSet
                        {
                            SetDetails = x.ToFormattedMetric(action.MetricType)
                        }).ToImmutableArray()
            }).ToImmutableArray()
        };
    }

    private static string ToMeasureTotalVolume(this IEnumerable<ExerciseSetMetrics> sets, MetricType metricType)
        => (metricType switch
        {
            MetricType.Weight => $"V: {sets.Select(x => x.Weight * x.Reps).Sum()}",
            MetricType.Time => $"T: {sets.Select(x => x.Time).Sum()}",
            MetricType.TimeAndDistance => $"D: {sets.Select(x => x.Distance).Sum()}",
            _ => ""
        }).WithFormattedMetricMeasureMetric(metricType);
}

public class HistoryStateEffects
{
    private readonly IClientStorage _clientStorage;

    public HistoryStateEffects(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnSetSelectedExerciseAction(SetSelectedExerciseAction action, IDispatcher dispatcher)
    {
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        var exercise = workout!.Exercises.Single(x => x.Id == action.ExerciseId);
        var stats = await _clientStorage.ExerciseStatistics.FindOrDefaultByIdAsync(exercise.Exercise.Id);

        dispatcher.Dispatch(new SetHistoryAction(stats, exercise.Exercise.MetricType));
    }
}
