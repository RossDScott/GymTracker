using GymTracker.Domain.Models;
using GymTracker.LocalStorage;
using GymTracker.LocalStorage.Core;

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

        var exercises = completedWorkouts
            .OrderByDescending(x => x.WorkoutEnd)
            .SelectMany(wo => wo.Exercises
            .Select(ex => new
            {
                ExerciseId = ex.Exercise.Id,
                WorkoutEnd = wo.WorkoutEnd!.Value,
                Sets = ex.Sets
            }))
            .GroupBy(x => x.ExerciseId)
            .ToList();

        var statistics = exercises
            .Select(exercise => new ExerciseStatistic
            {
                ExerciseId = exercise.Key,
                Logs = exercise.Select(completedExercise => new ExerciseLog
                {
                    WorkoutDateTime = completedExercise.WorkoutEnd,
                    Sets = completedExercise.Sets
                        .Where(x => x.SetType == "Set")
                        .Select(x => x.Metrics)
                        .ToList()
                }).ToList()
            }).ToList();

        _localStorageContex.ExerciseStatistics.SetAsync(statistics);
    }
}
