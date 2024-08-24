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
        _localStorageContex.Exercises.SubscribeToChanges(async _ =>
        {
            var workouts = await _localStorageContex.Workouts.GetOrDefaultAsync();
            await WorkoutsChanged(workouts);
        });
    }

    public async Task WorkoutsChanged(ICollection<Workout> workouts)
    {
        var exercises = await _localStorageContex.Exercises.GetOrDefaultAsync();
        var completedWorkouts = workouts
                                .Where(x => x.WorkoutEnd != null)
                                .OrderByDescending(x => x.WorkoutEnd)
                                .ToList();

        var completedExercises = completedWorkouts
            .SelectMany(wo => wo.Exercises
            .Select(ex => new
            {
                ExerciseId = ex.Exercise.Id,
                Name = ex.Exercise.Name,
                WorkoutEnd = wo.WorkoutEnd!.Value,
                ex.Sets,
                ShowChartOnHomePage = ex.Exercise.ShowChartOnHomepage
            }))
            .GroupBy(x => x.ExerciseId)
            .Select(completedExercise =>
            {
                var exercise = exercises.Single(x => x.Id == completedExercise.Key);
                return new ExerciseStatistic
                {
                    ExerciseId = completedExercise.Key,
                    ExerciseName = exercise.Name,
                    Logs = completedExercise
                        .Select(completedExercise => new ExerciseLog
                        {
                            WorkoutDateTime = completedExercise.WorkoutEnd,
                            Sets = completedExercise.Sets
                                .Where(x => x.SetType == DefaultData.SetType.Set && x.Completed)
                                .Select(x => x.Metrics)
                                .ToList(),
                            TotalVolume = completedExercise.Sets
                                .Where(x => x.Completed)
                                .Select(x => x.Metrics)
                                .GetTotalVolume(exercise.MetricType)
                        })
                        .Where(x => x.Sets.Any())
                        .ToList(),
                    ShowChartOnHomePage = exercise.ShowChartOnHomepage,
                    ExerciseMetric = exercise.MetricType.ToMetricDescription()
                };
            }).ToList();

        await _localStorageContex.ExerciseStatistics.SetAsync(completedExercises);

        var sixMonthsAgo = DateTimeOffset.Now.AddMonths(-6);
        var workoutPlanStatistics = completedWorkouts
            .GroupBy(x => x.Plan.Id)
            .Where(x => x.Any())
            .Select(plan => new WorkoutPlanStatistic
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
