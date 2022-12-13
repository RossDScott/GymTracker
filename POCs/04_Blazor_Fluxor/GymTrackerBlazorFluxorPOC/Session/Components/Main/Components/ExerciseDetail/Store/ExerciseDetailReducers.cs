using Fluxor;
using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.Components.Main.Components.ExerciseDetail.Store;

public static class ExerciseDetailReducers
{
    [ReducerMethod]
    public static ExerciseDetailState OnSetExerciseDetailAction(ExerciseDetailState state, SetExerciseDetailAction action)
    {
        return state with
        {
            SessionExerciseId = action.SessionExercise.Id,
            MetricType = action.SessionExercise.Exercise.MetricType,
            Sets = action.SessionExercise.Sets.Select(x =>
                    new Set
                    {
                        Id = x.Id,
                        Name = x.TargetSet.Name,
                        TargetReps = x.TargetSet.Reps,
                        TargetTime = x.TargetSet.Time,
                        TargetWeight = x.TargetSet.Weight,
                        Completed = false
                    }).ToImmutableList()
        };
    }

    [ReducerMethod]
    public static ExerciseDetailState OnSetSelectedSetAction(ExerciseDetailState state, SetSelectedSetAction action)
    {
        return state with { SelectedSetId = action.Id };
    }

    [ReducerMethod]
    public static ExerciseDetailState OnHandleToggleCompletedAction(ExerciseDetailState state, ToggleSetCompletedAction action)
    {
        var originalSet = state.Sets.Single(x => x.Id == action.Id);
        var updatedSet = originalSet with 
        { 
            Completed = !originalSet.Completed,
            ActualReps = originalSet.ActualReps ?? originalSet.TargetReps,
            ActualWeight = originalSet.ActualWeight ?? originalSet.TargetWeight,
            ActualTime = originalSet.ActualTime ?? originalSet.TargetTime
        };
        var sets = state.Sets.Replace(originalSet, updatedSet);

        return state with { Sets = sets };
    }

    [ReducerMethod]
    public static ExerciseDetailState OnHandleSetSetDataAction(ExerciseDetailState state, SetSetDataAction action)
    {
        var editSet = action.EditSet;
        var originalSet = state.Sets.Single(x => x.Id == editSet.Id);
        var updatedSet = originalSet with
        {
            TargetReps = editSet.TargetReps,
            TargetWeight = editSet.TargetWeight,
            TargetTime = editSet.TargetTime,

            ActualReps = editSet.ActualReps,
            ActualWeight = editSet.ActualWeight,
            ActualTime = editSet.ActualTime
        };
        var sets = state.Sets.Replace(originalSet, updatedSet);

        return state with { Sets = sets };
    }
}
