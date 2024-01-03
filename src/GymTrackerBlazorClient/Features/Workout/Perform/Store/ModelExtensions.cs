namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

using GymTracker.BlazorClient.Features.Common;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

public static class ModelExtensions
{
    public static WorkoutDetail ToWorkoutDetail(this Models.Workout workout)
        => new WorkoutDetail
        {
            Id = workout.Id,
            WorkoutStart = workout.WorkoutStart,
            WorkoutEnd = workout.WorkoutEnd,
            ExerciseList = workout.Exercises
            .OrderBy(x => x.Order)
            .Select(x => new ListItem(x.Id, x.Exercise.Name, false))
            .ToImmutableArray()
        };
}
