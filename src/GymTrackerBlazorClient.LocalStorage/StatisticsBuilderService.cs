using GymTracker.Domain.Models;
using GymTracker.LocalStorage;
using GymTracker.Repository;

namespace GymTracker.BlazorClient.Services;

public class StatisticsBuilderService : ITrigger
{
    private readonly ClientStorageContext _localStorageContex;

    public StatisticsBuilderService(ClientStorageContext localStorageContex)
    {
        _localStorageContex = localStorageContex;
    }

    public void Subscribe()
    {
        _localStorageContex.Workouts.SubscribeToChanges(WorkoutsChanged);
    }

    public void WorkoutsChanged(ICollection<Workout> workouts)
    {
        var completedWorkouts = workouts.Where(x => x.WorkoutEnd != null).ToList();

        completedWorkouts
            .SelectMany(wo => wo.Exercises.Select(ex => new { wo.WorkoutEnd, ex.Exercise }))
            .GroupBy(x => x.Exercise.Id)
            .ToList()
            .ForEach(x =>
            {

            });


    }
}
