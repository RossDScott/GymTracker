using Fluxor;
using GymTracker.Domain.Models.Extensions;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

public static class WorkoutHistoryReducers
{
    [ReducerMethod]
    public static WorkoutHistoryState OnSetWorkoutPlanId(WorkoutHistoryState state, SetWorkoutPlanIdAction action)
        => state with { SelectedWorkoutPlanId = action.Id };

    [ReducerMethod]
    public static WorkoutHistoryState OnSetWorkoutHistory(WorkoutHistoryState state, SetWorkoutHistoryAction action)
    {
        var sixMonthsAgo = DateTimeOffset.Now.AddMonths(-6);
        var workouts = action.Workouts
                                .Where(x => x.WorkoutEnd > sixMonthsAgo)
                                .ToList();

        var exercises = workouts
                            .SelectMany(x => x.Exercises.Select(x => x.Exercise))
                            .DistinctBy(x => x.Id)
                            .ToList();

        var workoutExercises = workouts
                                .Where(x => x.WorkoutEnd != null)
                                .SelectMany(wo => wo.Exercises
                                                    .Select(x => new { WorkoutEnd = DateOnly.FromDateTime(wo.WorkoutEnd!.Value.Date), Exercise = x }))
                                .OrderByDescending(x => x.WorkoutEnd)
                                .ToList();

        return state with
        {
            Dates = workoutExercises
                    .Select(x => x.WorkoutEnd)
                    .Distinct()
                    .OrderDescending()
                    .ToImmutableArray(),
            Exercises = exercises
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
                                                TotalVolume = woe.Exercise.Sets
                                                                          .Select(x => x.Metrics)
                                                                          .GetTotalVolumeWithMeasure(woe.Exercise.Exercise.MetricType)
                                            }).ToImmutableArray(),
                                SetNames = workoutExercises
                                            .Where(x => x.Exercise.Exercise.Id == exercise.Id)
                                            .SelectMany(x => x.Exercise.Sets)
                                            .DistinctBy(x => x.Order)
                                            .OrderBy(x => x.Order)
                                            .Select(x => x.ToSetTypeAndSequence())
                                            .ToImmutableArray()
                            }).ToImmutableArray()
        };
    }
}
