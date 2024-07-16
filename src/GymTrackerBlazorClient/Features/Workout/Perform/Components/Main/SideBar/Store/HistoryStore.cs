using Fluxor;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models;
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

    public ImmutableList<Workout> History { get; init; } = ImmutableList<Workout>.Empty;
}

public record Workout
{
    public DateTimeOffset CompletedOn { get; init; }
    public required ImmutableList<ExerciseSetMetrics> Sets { get; init; }
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
                History = ImmutableList<Workout>.Empty
            };

        var sets = dataForMonths.SelectMany(x => x.Sets).ToList();

        return state with
        {
            MetricType = action.MetricType,
            MaxSet = action.MetricType switch
            {
                MetricType.Weight => sets.MaxBy(x => x.Weight),
                MetricType.Time => sets.MaxBy(x => x.Time),
                MetricType.TimeAndDistance => sets.MaxBy(x => x.Distance),
                _ => null
            },
            MaxVolume = action.MetricType switch
            {
                MetricType.Weight => sets.MaxBy(x => x.Weight * x.Reps),
                _ => null
            },
            History = dataForMonths
            .OrderByDescending(x => x.WorkoutDateTime)
            .Select(x => new Workout
            {
                CompletedOn = x.WorkoutDateTime,
                Sets = x.Sets.ToImmutableList()
            }).ToImmutableList()
        };
    }
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
