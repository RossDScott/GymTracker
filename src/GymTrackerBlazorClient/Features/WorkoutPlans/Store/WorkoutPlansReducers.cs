using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public static class WorkoutPlansReducers
{
    [ReducerMethod]
    public static WorkoutPlansState OnSetWorkoutPlans(WorkoutPlansState state, SetWorkoutPlansAction action) =>
        state with
        {
            WorkoutPlans = action.WorkoutPlans
                .Select(x => new ListItem(x.Id, x.Name, x.IsAcitve))
                .ToImmutableArray()
        };

    [ReducerMethod]
    public static WorkoutPlansState OnCreateNewExercise(WorkoutPlansState state, CreateNewWorkoutPlanAction action) =>
        state with
        {
            SelectedWorkoutPlan = new WorkoutPlanDetail()
        };

    [ReducerMethod]
    public static WorkoutPlansState OnSetWorkoutPlan(WorkoutPlansState state, SetWorkoutPlanAction action) =>
        state with
        {
            SelectedWorkoutPlan = action.WorkoutPlan?.ToDetailItem(),
            SelectedExercise = null

        };

    [ReducerMethod]
    public static WorkoutPlansState OnSetExercise(WorkoutPlansState state, SetExerciseAction action)
        => state with
        {
            SelectedExercise = action.Exercise.ToDetailItem()
        };
}
