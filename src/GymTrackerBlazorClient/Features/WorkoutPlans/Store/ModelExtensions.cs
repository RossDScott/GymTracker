using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public static class ModelExtensions
{
    public static WorkoutPlanDetail ToDetailItem(this WorkoutPlan plan)
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

    public static PlannedExerciseDetail ToDetailItem(this PlannedExercise exercise)
        => new PlannedExerciseDetail
        {
            Id = exercise.Id,
            Name = exercise.Exercise.Name,
            MetricType = exercise.Exercise.MetricType,
            AutoTriggerRestTimer = exercise.AutoTriggerRestTimer,
            RestInterval = exercise.RestInterval,
            PlannedSets = exercise.PlannedSets
                                    .OrderBy(x => x.Order)
                                    .Select(x => x.ToListItem())
                                    .ToImmutableArray()
        };

    public static ListItem ToListItem(this PlannedExercise plannedExercise)
        => new ListItem(plannedExercise.Id, plannedExercise.Exercise.Name, true);

    public static PlannedSetDetail ToListItem(this PlannedExerciseSet set)
        => new PlannedSetDetail
        {
            Id = set.Id,
            Order = set.Order,
            SetType = set.SetType,
            OrderForSetType = set.OrderForSetType,
            Distance = set.TargetMetrics.Distance,
            Reps = set.TargetMetrics.Reps,
            Time = set.TargetMetrics.Time,
            Weight = set.TargetMetrics.Weight
        };

    public static PlannedExerciseSet ToModel(this PlannedSetDetail set)
        => new PlannedExerciseSet
        {
            Id = set.Id,
            Order = set.Order,
            OrderForSetType = set.OrderForSetType,
            SetType = set.SetType,
            TargetMetrics = new ExerciseSetMetrics
            {
                Distance = set.Distance,
                Reps = set.Reps,
                Time = set.Time,
                Weight = set.Weight
            }
        };

    public static IList<PlannedExerciseSet> OrderSetTypes(this IList<PlannedExerciseSet> exerciseSets)
    {
        var dic = new Dictionary<string, int>();

        foreach (var exerciseSet in exerciseSets)
        {
            if(!dic.ContainsKey(exerciseSet.SetType))
                dic.Add(exerciseSet.SetType, 0);

            dic[exerciseSet.SetType] += 1;
            exerciseSet.OrderForSetType = dic[exerciseSet.SetType];
        }

        return exerciseSets;
    }
}
