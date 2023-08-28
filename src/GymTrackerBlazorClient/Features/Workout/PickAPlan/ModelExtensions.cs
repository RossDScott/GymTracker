using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.PickAPlan;

public static class ModelExtensions
{
    public static WorkoutPlanListItem ToWorkoutPlanListItem(this WorkoutPlan workoutPlan) =>
        new WorkoutPlanListItem
        {
            Id = workoutPlan.Id,
            Name = workoutPlan.Name,
            LastPerformedOn = null,
            PlannedExercises = workoutPlan.PlannedExercises
                .Select(x => x.Exercise.Name)
                .ToImmutableArray()
        };
}
