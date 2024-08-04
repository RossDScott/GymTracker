using Fluxor;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.Main.ExerciseDetail.Store;

public static class ExerciseDetailReducers
{
    [ReducerMethod]
    public static ExerciseDetailState OnSetExerciseDetailAction(ExerciseDetailState state, SetExerciseDetailAction action)
    {
        return state with
        {
            WorkoutExerciseId = action.WorkoutExercise.Id,
            MetricType = action.WorkoutExercise.Exercise.MetricType,
            Sets = action.WorkoutExercise.Sets.Select(x =>
                    new Set
                    {
                        Id = x.Id,
                        Name = x.PlannedExerciseSet?.SetType ?? string.Empty,

                        TargetReps = x.PlannedExerciseSet?.TargetMetrics?.Reps,
                        TargetTime = x.PlannedExerciseSet?.TargetMetrics?.Time,
                        TargetWeight = x.PlannedExerciseSet?.TargetMetrics?.Weight,

                        ActualReps = x.Metrics.Reps,
                        ActualTime = x.Metrics.Time,
                        ActualWeight = x.Metrics.Weight,

                        Completed = x.Completed
                    }).ToImmutableList(),
            SelectedSetId = state.WorkoutExerciseId == action.WorkoutExercise.Id
                ? state.SelectedSetId
                : null
        };
    }

    [ReducerMethod]
    public static ExerciseDetailState OnSetSelectedSetAction(ExerciseDetailState state, SetSelectedSetAction action)
    {
        var sets = state.Sets;
        var currentlySelectedSet = sets.SingleOrDefault(x => x.Id == state.SelectedSetId);

        if (currentlySelectedSet?.Id == action.Id)
            return state with { SelectedSetId = null };

        if (currentlySelectedSet != null && !currentlySelectedSet.Completed)
            sets = sets.Replace(currentlySelectedSet, currentlySelectedSet with
            {
                ActualReps = null,
                ActualTime = null,
                ActualWeight = null
            });

        if (action.Id == null)
            return state with { SelectedSetId = null, Sets = sets };

        var newSelectedSet = sets.Single(x => x.Id == action.Id);
        sets = sets.Replace(newSelectedSet, newSelectedSet with
        {
            ActualReps = newSelectedSet.ActualReps ?? newSelectedSet.TargetReps,
            ActualTime = newSelectedSet.ActualTime ?? newSelectedSet.TargetTime,
            ActualWeight = newSelectedSet.ActualWeight ?? newSelectedSet.TargetWeight
        });

        return state with
        {
            SelectedSetId = action.Id,
            Sets = sets
        };
    }
}
