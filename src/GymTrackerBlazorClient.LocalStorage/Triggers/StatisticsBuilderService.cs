using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;

namespace GymTracker.LocalStorage.Triggers;

public class StatisticsBuilderService : ITrigger
{
    private readonly ClientStorageContext _localStorageContex;

    public StatisticsBuilderService(ClientStorageContext localStorageContex)
    {
        _localStorageContex = localStorageContex;
    }

    public void Subscribe()
    {
        // Item-level upsert: incremental update for single workout saves
        _localStorageContex.Workouts.SubscribeToItemUpsert(WorkoutUpserted);

        // Collection-level changes: full rebuild for bulk operations (migration, restore)
        _localStorageContex.Workouts.SubscribeToChanges(WorkoutsChanged);

        _localStorageContex.Exercises.SubscribeToChanges(async _ =>
        {
            var workouts = await _localStorageContex.Workouts.GetOrDefaultAsync();
            await WorkoutsChanged(workouts);
        });
    }

    /// <summary>
    /// Incremental update when a single workout is upserted.
    /// Only updates statistics for the affected workout, plan, and exercises.
    /// </summary>
    public async Task WorkoutUpserted(Workout workout)
    {
        if (workout.WorkoutEnd is null)
            return; // Not a completed workout, nothing to compute

        var exercises = await _localStorageContex.Exercises.GetOrDefaultAsync();

        // 1. Upsert the single WorkoutStatistic
        var workoutStat = workout.ToWorkoutStatistics();
        await _localStorageContex.WorkoutStatistics.UpsertAsync(workoutStat);

        // 2. Update ExerciseStatistics for each exercise in this workout
        foreach (var workoutExercise in workout.Exercises)
        {
            var exercise = exercises.SingleOrDefault(x => x.Id == workoutExercise.Exercise.Id);
            if (exercise is null) continue;

            var existingStat = await _localStorageContex.ExerciseStatistics
                .FindOrDefaultByIdAsync(workoutExercise.Exercise.Id);

            var newLog = new ExerciseLog
            {
                WorkoutDateTime = workout.WorkoutEnd.Value,
                Sets = workoutExercise.Sets
                    .Where(x => x.SetType == DefaultData.SetType.Set && x.Completed)
                    .Select(x => x.Metrics)
                    .ToList(),
                TotalVolume = workoutExercise.Sets
                    .Where(x => x.Completed)
                    .Select(x => x.Metrics)
                    .GetTotalVolume(exercise.MetricType)
            };

            if (!newLog.Sets.Any())
                continue;

            var logs = existingStat?.Logs?.ToList() ?? new List<ExerciseLog>();

            // Replace existing log for this workout date, or add new
            var existingLogIndex = logs.FindIndex(l => l.WorkoutDateTime == workout.WorkoutEnd.Value);
            if (existingLogIndex >= 0)
                logs[existingLogIndex] = newLog;
            else
                logs.Add(newLog);

            var updatedStat = new ExerciseStatistic
            {
                ExerciseId = exercise.Id,
                ExerciseName = exercise.Name,
                Logs = logs,
                ShowChartOnHomePage = exercise.ShowChartOnHomepage,
                ExerciseMetric = exercise.MetricType.ToMetricDescription()
            };

            await _localStorageContex.ExerciseStatistics.UpsertAsync(updatedStat);
        }

        // 3. Update WorkoutPlanStatistic for this workout's plan
        var existingPlanStat = await _localStorageContex.WorkoutPlanStatistics
            .FindOrDefaultByIdAsync(workout.Plan.Id);

        var sixMonthsAgo = DateTimeOffset.Now.AddMonths(-6);
        var currentVolume = workout.GetWeightTotalVolume();

        var bestVolume = existingPlanStat?.BestWeightTotalVolumeIn6Months ?? 0m;
        if (workout.WorkoutEnd > sixMonthsAgo && currentVolume > bestVolume)
            bestVolume = currentVolume;

        var planStat = new WorkoutPlanStatistic
        {
            WorkoutPlanId = workout.Plan.Id,
            PreviousWorkout = workoutStat,
            BestWeightTotalVolumeIn6Months = bestVolume
        };

        await _localStorageContex.WorkoutPlanStatistics.UpsertAsync(planStat);
    }

    /// <summary>
    /// Full rebuild of all statistics. Used for bulk operations (migration, restore, exercise changes).
    /// </summary>
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
                ex.Exercise.Name,
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
