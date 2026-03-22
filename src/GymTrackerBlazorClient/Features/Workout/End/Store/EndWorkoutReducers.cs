using Fluxor;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.Domain.Models.Statistics;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.End.Store;

public static class EndWorkoutReducers
{
    [ReducerMethod]
    public static EndWorkoutState OnSetEndWorkout(EndWorkoutState state, SetEndWorkoutAction action)
    {
        return state with
        {
            WorkoutStartDate = action.Workout.WorkoutStart.Date,
            WorkoutStartTime = action.Workout.WorkoutStart.TimeOfDay,
            WorkoutEndDate = DateTimeOffset.Now.Date,
            WorkoutEndTime = DateTimeOffset.Now.TimeOfDay,
            TotalVolumeMessage = action.Workout.Exercises
                .Where(x => x.Exercise.MetricType == MetricType.Weight)
                .SelectMany(x => x.Sets)
                .Select(x => x.Metrics)
                .GetWeightTotalVolumeWithMeasure(),
            ExerciseList = action.Workout.Exercises
                .Select(exercise =>
                {
                    var exerciseMilestones = action.ExerciseMilestones
                        .FirstOrDefault(m => m.ExerciseId == exercise.Exercise.Id);

                    return new ExerciseDetail
                    {
                        WorkoutExerciseId = exercise.Id,
                        PlannedWorkoutExerciseId = exercise.PlannedExercise?.Id,
                        ExerciseName = exercise.Exercise.Name,
                        MetricType = exercise.Exercise.MetricType,
                        ProgressSets = BuildProgressSets(exercise),
                        CompletedSets = exercise.Sets
                                                .Where(x => x.SetType == DefaultData.SetType.Set)
                                                .OrderBy(x => x.CompletedOn)
                                                .Select(x => x.Metrics).ToImmutableArray(),
                        Milestones = exerciseMilestones?.SetMilestones.ToImmutableArray()
                                     ?? ImmutableArray<SetMilestone>.Empty,
                        VolumeMilestone = exerciseMilestones?.VolumeMilestone
                    };
                }).ToImmutableArray(),
            PreviousStatistics = action.PreviousStatistics,
            HasVolumePR = ComputeHasVolumePR(action)
        };
    }

    private static bool ComputeHasVolumePR(SetEndWorkoutAction action)
    {
        if (action.PreviousStatistics is null)
            return false;

        var currentVolume = action.Workout.Exercises
            .Where(x => x.Exercise.MetricType == MetricType.Weight)
            .SelectMany(x => x.Sets)
            .Where(x => x.Completed)
            .Sum(x => (x.Metrics.Weight ?? 0) * (x.Metrics.Reps ?? 0));

        return currentVolume > 0 && currentVolume > action.PreviousStatistics.BestWeightTotalVolumeIn6Months;
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

    [ReducerMethod]
    public static EndWorkoutState OnSetCustomProgress(EndWorkoutState state, SetCustomProgressAction action)
    {
        var exercise = state.ExerciseList.First(x => x.WorkoutExerciseId == action.WorkoutExerciseId);
        var progressSets = exercise.ProgressSets.ToList();

        progressSets.ForEach(x => x.Selected = false);

        var existing = progressSets.SingleOrDefault(x => x.ProgressType == ProgressType.Custom);
        if (existing != null)
            progressSets.Remove(existing);

        progressSets.Add(new ProgressSet
        {
            ProgressType = ProgressType.Custom,
            Metrics = action.Metrics,
            Selected = true
        });

        return state with
        {
            ExerciseList = state.ExerciseList.Replace(
                exercise,
                exercise with { ProgressSets = progressSets.ToImmutableArray() })
        };
    }

    private static ImmutableArray<ProgressSet> BuildProgressSets(WorkoutExercise workoutExercise)
    {
        var metricType = workoutExercise.Exercise.MetricType;
        var progressSets = new List<ProgressSet>();
        var completedAll = workoutExercise.Sets.All(x => x.Completed);
        var completedAny = workoutExercise.Sets.Any(x => x.Completed);

        if (workoutExercise.PlannedExercise != null)
        {
            var previousSet = workoutExercise.PlannedExercise.GetMaxSet();
            if (previousSet != null)
                progressSets.Add(new ProgressSet { ProgressType = ProgressType.Previous, Metrics = previousSet, Selected = false });
        }

        var maxSet = workoutExercise.GetMaxSet();
        var minSet = workoutExercise.GetMinSet();
        if (maxSet != null && minSet != null)
        {
            progressSets.Add(new ProgressSet { ProgressType = ProgressType.MaxSet, Metrics = maxSet, Selected = false });

            if (completedAll && workoutExercise.PlannedExercise != null)
            {
                ExerciseSetMetrics? progressSet = null;

                if (metricType == MetricType.Weight)
                {
                    if (minSet.Reps >= workoutExercise.PlannedExercise.TargetRepsUpper)
                    {
                        progressSet = minSet with
                        {
                            Reps = workoutExercise.PlannedExercise.TargetRepsLower,
                            Weight = minSet.Weight + workoutExercise.PlannedExercise.TargetWeightIncrement
                        };
                    }
                    else
                    {
                        progressSet = minSet with { Reps = minSet.Reps + 1 };
                    }
                }

                if (metricType == MetricType.Reps)
                {
                    progressSet = minSet with { Reps = minSet.Reps + 1 };
                }

                if (progressSet != null)
                    progressSets.Add(new ProgressSet { ProgressType = ProgressType.AutoProgress, Metrics = progressSet, Selected = true });
            }
        }

        if (!progressSets.Any(x => x.Selected))
        {
            ProgressSet? selectedProgress = progressSets.FirstOrDefault(x => x.ProgressType == ProgressType.MaxSet);
            var previousProgress = progressSets.FirstOrDefault(x => x.ProgressType == ProgressType.Previous);

            if (selectedProgress != null && previousProgress != null &&
                selectedProgress.Metrics.ToStandardMeasure(metricType) < previousProgress.Metrics.ToStandardMeasure(metricType))
                selectedProgress = previousProgress;

            if (!completedAny)
                selectedProgress = previousProgress;

            selectedProgress ??= previousProgress;

            if (selectedProgress != null)
                selectedProgress.Selected = true;
        }

        return progressSets.ToImmutableArray();
    }
}
