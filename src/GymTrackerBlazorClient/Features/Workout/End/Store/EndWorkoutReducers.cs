﻿using Fluxor;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

public static class EndWorkoutReducers
{
    [ReducerMethod]
    public static EndWorkoutState OnSetEndWorkout(EndWorkoutState state, SetEndWorkoutAction action)
    {
        return state with
        {
            Duration = DateTimeOffset.Now - action.Workout.WorkoutStart,
            TotalVolume = action.Workout.Exercises
                .Where(x => x.Exercise.MetricType == MetricType.Weight)
                .SelectMany(x => x.Sets)
                .Sum(x => (x.Metrics.Weight ?? 0) * (x.Metrics.Reps ?? 0)),
            ExerciseList = action.Workout.Exercises
                .Select(exercise => new ExerciseDetail
                {
                    WorkoutExerciseId = exercise.Id,
                    PlannedWorkoutExerciseId = exercise.PlannedExercise?.Id,
                    ExerciseName = exercise.Exercise.Name,
                    MetricType = exercise.Exercise.MetricType,
                    ProgressSets = BuildProgressSets(exercise)
                }).ToImmutableArray()
        };
    }

    [ReducerMethod]
    public static EndWorkoutState OnSetSelectedProgress(EndWorkoutState state, SetSelectedProgressAction action)
    {
        var exercise = state.ExerciseList.First(x => x.WorkoutExerciseId == action.workoutExerciseId);
        var progressSets = exercise.ProgressSets.ToList();
        var selected = progressSets.SingleOrDefault(x => x.ProgressType == action.progressType);

        if (selected == null)
            return state;

        progressSets.ForEach(x => x.Selected = false);
        selected.Selected = true;

        return state with
        {
            ExerciseList = state.ExerciseList
                                .Replace(
                                    exercise,
                                    exercise with
                                    { ProgressSets = progressSets.ToImmutableArray() })
        };
    }

    private static ImmutableArray<ProgressSet> BuildProgressSets(WorkoutExercise workoutExercise)
    {
        var metricType = workoutExercise.Exercise.MetricType;
        var progressSets = new List<ProgressSet>();

        if (workoutExercise.PlannedExercise != null)
        {
            var previousSet = workoutExercise.PlannedExercise.GetMaxSet();
            if (previousSet != null)
                progressSets.Add(new ProgressSet { ProgressType = ProgressType.Previous, Metrics = previousSet, Selected = false });
        }

        var maxSet = workoutExercise.GetMaxSet();
        if (maxSet != null)
        {
            progressSets.Add(new ProgressSet { ProgressType = ProgressType.MaxSet, Metrics = maxSet, Selected = false });

            if (metricType == MetricType.Weight &&
                workoutExercise.PlannedExercise != null &&
                maxSet.Reps >= workoutExercise.PlannedExercise.TargetRepsUpper)
            {
                var progressSet = maxSet with
                {
                    Reps = workoutExercise.PlannedExercise.TargetRepsLower,
                    Weight = maxSet.Weight + workoutExercise.PlannedExercise.TargetWeightIncrement
                };
                progressSets.Add(new ProgressSet { ProgressType = ProgressType.AutoProgress, Metrics = progressSet, Selected = true });
            }
        }

        if (!progressSets.Any(x => x.Selected))
        {
            ProgressSet? selectedProgress = progressSets.FirstOrDefault(x => x.ProgressType == ProgressType.MaxSet);
            var previousProgress = progressSets.FirstOrDefault(x => x.ProgressType == ProgressType.Previous);

            if (selectedProgress != null && previousProgress != null &&
                selectedProgress.Metrics.GetMeasure(metricType) < previousProgress.Metrics.GetMeasure(metricType))
                selectedProgress = previousProgress;

            selectedProgress ??= previousProgress;

            if (selectedProgress != null)
                selectedProgress.Selected = true;
        }

        return progressSets.ToImmutableArray();
    }


}
