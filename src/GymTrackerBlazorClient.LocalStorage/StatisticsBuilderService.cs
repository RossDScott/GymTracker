using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.Domain.Models.Statistics;
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

    public async Task WorkoutsChanged(ICollection<Workout> workouts)
    {
        var completedWorkouts = workouts
                                .Where(x => x.WorkoutEnd != null)
                                .OrderByDescending(x => x.WorkoutEnd)
                                .ToList();

        var exercises = completedWorkouts
            .SelectMany(wo => wo.Exercises
            .Select(ex => new
            {
                ExerciseId = ex.Exercise.Id,
                WorkoutEnd = wo.WorkoutEnd!.Value,
                Sets = ex.Sets
            }))
            .GroupBy(x => x.ExerciseId)
            .ToList();

        var exerciseStatistics = exercises
            .Select(exercise => new ExerciseStatistic
            {
                ExerciseId = exercise.Key,
                Logs = exercise.Select(completedExercise => new ExerciseLog
                {
                    WorkoutDateTime = completedExercise.WorkoutEnd,
                    Sets = completedExercise.Sets
                        .Where(x => x.SetType == DefaultData.SetType.Set && x.Completed)
                        .Select(x => x.Metrics)
                        .ToList()
                })
                .Where(x => x.Sets.Any())
                .ToList()
            }).ToList();

        await _localStorageContex.ExerciseStatistics.SetAsync(exerciseStatistics);

        var sixMonthsAgo = DateTimeOffset.Now.AddMonths(-6);
        var workoutPlanStatistics = completedWorkouts
            .GroupBy(x => x.Plan.Id)
            .Where(x => x.Any())
            .Select(plan => new WorkoutPlanStatistics
            {
                WorkoutPlanId = plan.Key,
                PreviousWorkout = plan.First().ToWorkoutStatistics(),
                BestWeightTotalVolumeIn6Months =
                    plan
                        .Where(x => x.WorkoutEnd > sixMonthsAgo)
                        .Select(x => x.GetWeightTotalVolume())
                        .DefaultIfEmpty()
                        .Max()
            })
            .ToList();
        await _localStorageContex.WorkoutPlanStatistics.SetAsync(workoutPlanStatistics);

        var workoutStatistics = completedWorkouts
                                    .Select(x => x.ToWorkoutStatistics())
                                    .ToList();
        await _localStorageContex.WorkoutStatistics.SetAsync(workoutStatistics);
    }
}
