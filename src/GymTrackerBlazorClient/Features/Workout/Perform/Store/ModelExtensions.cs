namespace GymTracker.BlazorClient.Features.Workout.Perform.Store;

using Models = GymTracker.Domain.Models;

public static class ModelExtensions
{
    public static WorkoutDetail ToWorkoutDetail(this Models.Workout workout)
        => new WorkoutDetail
        {
            Id = workout.Id,
            WorkoutStart = workout.WorkoutStart,
            WorkoutEnd = workout.WorkoutEnd
        };
}
