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

    private static WorkoutPlanDetail ToDetailItem(this WorkoutPlan plan) 
        => new WorkoutPlanDetail
        {
            Id = plan.Id,
            Name = plan.Name,
            PlannedExercises = plan.PlannedExercises
                                    .OrderBy(x => x.Order)                        
                                    .Select(x => x.ToListItem())
                                    .ToImmutableArray(),
            IsActive = plan.IsAcitve
        };

    private static PlannedExerciseDetail ToDetailItem(this PlannedExercise exercise)
        => new PlannedExerciseDetail
        {
            Id = exercise.Id,
            Name = exercise.Exercise.Name,
            AutoTriggerRestTimer = exercise.AutoTriggerRestTimer,
            RestInterval = exercise.RestInterval,
            PlannedSets = exercise.PlannedSets
                                    .OrderBy(x => x.Order)
                                    .Select(x => x.ToListItem())
                                    .ToImmutableArray()
        };

    private static ListItem ToListItem(this PlannedExercise plannedExercise) 
        => new ListItem(plannedExercise.Id, plannedExercise.Exercise.Name, true);

    private static ListItem ToListItem(this PlannedExerciseSet set)
        => new ListItem(set.Id, set.SetType, true);
}
