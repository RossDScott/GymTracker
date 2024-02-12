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
                        Completed = false
                    }).ToImmutableList(),
            SelectedSetId = state.WorkoutExerciseId == action.WorkoutExercise.Id
                ? state.SelectedSetId
                : null
        };
    }

    [ReducerMethod]
    public static ExerciseDetailState OnSetSelectedSetAction(ExerciseDetailState state, SetSelectedSetAction action)
    {
        return state with { SelectedSetId = action.Id };
    }
}
