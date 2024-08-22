using System.Collections.Immutable;
using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain.Models.Extensions;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

public static class WorkoutHistoryReducers
{
    [ReducerMethod]
    public static WorkoutHistoryState OnSetInitialData(WorkoutHistoryState state, SetInitialDataAction action)
    {
        var sixMonthsAgo = DateTimeOffset.Now.AddMonths(-6);
        var workouts = action.Workouts
                                .Where(x => x.WorkoutEnd.HasValue && x.WorkoutEnd > sixMonthsAgo)
                                .ToImmutableArray();

        var workoutPlans = action.WorkoutPlans
                                 .Select(x => new ListItem(x.Id, x.Name))
                                 .ToImmutableArray();

        state = state with
        {
            Initalised = true,
            WorkoutPlans = workoutPlans,
            SelectedWorkoutPlanId = workoutPlans.First().Id,
            Workouts = workouts,

        };

        return CalculateFilter(state);
    }

    [ReducerMethod]
    public static WorkoutHistoryState OnSetWorkoutPlanId(WorkoutHistoryState state, SetWorkoutPlanIdAction action)
        => CalculateFilter(state with { SelectedWorkoutPlanId = action.Id });

    [ReducerMethod]
    public static WorkoutHistoryState OnSetDateRange(WorkoutHistoryState state, SetDateRangeAction action)
        => CalculateFilter(state with { WorkoutDateRange = action.DateRange });

    [ReducerMethod]
    public static WorkoutHistoryState OnSetPage(WorkoutHistoryState state, SetPageAction action)
        => state with { SelectedPage = action.Page };

    private static WorkoutHistoryState CalculateFilter(WorkoutHistoryState state)
    {
        var filteredWorkouts = state.Workouts
                                    .Where(x =>
                                                x.WorkoutEnd > state.WorkoutDateRange.Start &&
                                                x.WorkoutEnd < state.WorkoutDateRange.End)
                                    .Where(x => x.Plan.Id == state.SelectedWorkoutPlanId)
                                    .OrderByDescending(x => x.WorkoutEnd)
                                    .ToList();

        var dates = filteredWorkouts
                        .SelectMany(wo => wo.Exercises
                                            .Select(x => wo.WorkoutEnd!.Value))
                        .Distinct()
                        .ToImmutableArray();

        var workoutExercises = filteredWorkouts
                        .SelectMany(wo => wo.Exercises
                                            .Select(x => new { WorkoutEnd = wo.WorkoutEnd!.Value, Exercise = x }))
                        .ToList();

        var exercises = filteredWorkouts
                        .SelectMany(x => x.Exercises.Select(x => x.Exercise))
                        .DistinctBy(x => x.Id)
                        .ToList();

        var filteredExercises = exercises
                            .Select(exercise => new Exercise
                            {
                                ExerciseName = exercise.Name,
                                Records = workoutExercises
                                            .Where(x => x.Exercise.Exercise.Id == exercise.Id)
                                            .Select(woe => new ExerciseRecord
                                            {
                                                Date = woe.WorkoutEnd,
                                                Sets = woe.Exercise.Sets
                                                                   .Select(s => new Set
                                                                   {
                                                                       SetName = s.ToSetTypeAndSequence(),
                                                                       Measure = s.Metrics.ToFormattedMetric(woe.Exercise.Exercise.MetricType)
                                                                   }).ToImmutableArray(),
                                                TotalVolumeWithMeasure = woe.Exercise.Sets
                                                                          .Select(x => x.Metrics)
                                                                          .GetTotalVolumeWithMeasure(woe.Exercise.Exercise.MetricType),
                                                TotalVolume = woe.Exercise.Sets
                                                    .Select(x => x.Metrics)
                                                    .GetTotalVolume(woe.Exercise.Exercise.MetricType)
                                            }).ToImmutableArray(),
                                SetNames = workoutExercises
                                            .Where(x => x.Exercise.Exercise.Id == exercise.Id)
                                            .SelectMany(x => x.Exercise.Sets)
                                            .DistinctBy(x => x.Order)
                                            .OrderBy(x => x.Order)
                                            .Select(x => x.ToSetTypeAndSequence())
                                            .ToImmutableArray()
                            }).ToImmutableArray();

        return state with { Dates = dates, FilteredExercises = filteredExercises };
    }
}