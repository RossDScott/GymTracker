using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;
using System.Collections.Immutable;

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
        _localStorageContex.Workouts.SubscribeToItemUpsert(async workout =>
        {
            await WorkoutUpserted(workout);
        });

        _localStorageContex.Exercises.SubscribeToItemUpsert(exercise =>
        {
            _ = RebuildAllStatistics();
            return Task.CompletedTask;
        });

        _localStorageContex.WorkoutPlans.SubscribeToItemUpsert(plan =>
        {
            _ = ComputeAndSaveNextWorkout();
            return Task.CompletedTask;
        });
    }

    private async Task WorkoutUpserted(Workout workout)
    {
        if (workout.WorkoutEnd is null)
            return;

        var exercises = await _localStorageContex.Exercises.GetOrDefaultAsync();

        var workoutStat = workout.ToWorkoutStatistics();

        foreach (var workoutExercise in workout.Exercises)
        {
            var exercise = exercises.SingleOrDefault(x => x.Id == workoutExercise.Exercise.Id);
            if (exercise is null) continue;

            var existingStat = await _localStorageContex.ExerciseStatistics
                .FindOrDefaultByIdAsync(workoutExercise.Exercise.Id);

            var completedSets = workoutExercise.Sets
                .Where(x => x.SetType == DefaultData.SetType.Set && x.Completed)
                .Select(x => x.Metrics)
                .ToList();

            var newLog = new ExerciseLog
            {
                WorkoutDateTime = workout.WorkoutEnd.Value,
                Sets = completedSets,
                TotalVolume = workoutExercise.Sets
                    .Where(x => x.Completed)
                    .Select(x => x.Metrics)
                    .GetTotalVolume(exercise.MetricType)
            };

            if (!newLog.Sets.Any())
                continue;

            var milestones = MilestoneDetector.DetectExerciseMilestones(
                exercise.Id,
                exercise.Name,
                exercise.MetricType,
                completedSets,
                newLog.TotalVolume,
                existingStat);

            if (milestones.SetMilestones.Count > 0 || milestones.VolumeMilestone != null)
            {
                var exerciseStat = workoutStat.Exercises
                    .FirstOrDefault(e => e.ExerciseName == exercise.Name);
                if (exerciseStat != null)
                    exerciseStat.HasPR = true;
            }

            var logs = existingStat?.Logs?.ToList() ?? new List<ExerciseLog>();

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

        if (workoutStat.TotalWeightVolume > 0)
        {
            var allWorkoutStats = await _localStorageContex.WorkoutStatistics.GetOrDefaultAsync();
            var maxHistoricalVolume = allWorkoutStats
                .Where(s => s.WorkoutPlanId == workout.Plan.Id)
                .Select(s => s.TotalWeightVolume)
                .DefaultIfEmpty(0)
                .Max();

            if (workoutStat.TotalWeightVolume > maxHistoricalVolume)
                workoutStat.HasVolumePR = true;
        }

        await _localStorageContex.WorkoutStatistics.UpsertAsync(workoutStat);

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

        await ComputeAndSaveNextWorkout();
    }

    private async Task RebuildAllStatistics()
    {
        var exercises = await _localStorageContex.Exercises.GetOrDefaultAsync();
        var workouts = await _localStorageContex.Workouts.GetOrDefaultAsync();
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

        var exerciseStatsById = completedExercises.ToDictionary(e => e.ExerciseId);
        var workoutStatistics = completedWorkouts
                                    .Select(x => x.ToWorkoutStatistics())
                                    .ToList();

        foreach (var workoutStat in workoutStatistics)
        {
            var workout = completedWorkouts.Single(w => w.Id == workoutStat.WorkoutId);
            foreach (var workoutExercise in workout.Exercises)
            {
                if (!exerciseStatsById.TryGetValue(workoutExercise.Exercise.Id, out var exerciseStat))
                    continue;

                var completedSets = workoutExercise.Sets
                    .Where(s => s.SetType == DefaultData.SetType.Set && s.Completed)
                    .Select(s => s.Metrics)
                    .ToList();

                if (!completedSets.Any()) continue;

                var exercise = exercises.Single(e => e.Id == workoutExercise.Exercise.Id);
                var historicalStat = new ExerciseStatistic
                {
                    ExerciseId = exerciseStat.ExerciseId,
                    ExerciseName = exerciseStat.ExerciseName,
                    Logs = exerciseStat.Logs
                        .Where(l => l.WorkoutDateTime < workoutStat.CompletedOn)
                        .ToList(),
                    ExerciseMetric = exerciseStat.ExerciseMetric
                };

                var totalVolume = workoutExercise.Sets
                    .Where(s => s.Completed)
                    .Select(s => s.Metrics)
                    .GetTotalVolume(exercise.MetricType);

                var milestones = MilestoneDetector.DetectExerciseMilestones(
                    exercise.Id,
                    exercise.Name,
                    exercise.MetricType,
                    completedSets,
                    totalVolume,
                    historicalStat);

                if (milestones.SetMilestones.Count > 0 || milestones.VolumeMilestone != null)
                {
                    var exerciseStatEntry = workoutStat.Exercises
                        .FirstOrDefault(e => e.ExerciseName == exercise.Name);
                    if (exerciseStatEntry != null)
                        exerciseStatEntry.HasPR = true;
                }
            }
        }

        foreach (var planGroup in workoutStatistics.GroupBy(w => w.WorkoutPlanId))
        {
            var planWorkouts = planGroup.OrderBy(w => w.CompletedOn).ToList();
            var maxVolumeSoFar = 0m;
            foreach (var ws in planWorkouts)
            {
                if (ws.TotalWeightVolume > 0 && ws.TotalWeightVolume > maxVolumeSoFar)
                {
                    ws.HasVolumePR = true;
                    maxVolumeSoFar = ws.TotalWeightVolume;
                }
                else
                {
                    maxVolumeSoFar = Math.Max(maxVolumeSoFar, ws.TotalWeightVolume);
                }
            }
        }

        await _localStorageContex.WorkoutStatistics.SetAsync(workoutStatistics);

        await ComputeAndSaveNextWorkout();
    }

    private async Task ComputeAndSaveNextWorkout()
    {
        var plans = await _localStorageContex.WorkoutPlans.GetOrDefaultAsync();
        var regularPlans = plans.Where(p => p.IsRegularRoutine && p.IsAcitve).ToList();

        if (!regularPlans.Any())
            return;

        var planStats = await _localStorageContex.WorkoutPlanStatistics.GetOrDefaultAsync();
        var statsByPlanId = planStats.ToDictionary(s => s.WorkoutPlanId);

        var nextPlan = regularPlans
            .OrderBy(p => statsByPlanId.TryGetValue(p.Id, out var stat)
                ? stat.PreviousWorkout.CompletedOn
                : DateTimeOffset.MinValue)
            .First();

        var lastCompleted = statsByPlanId.TryGetValue(nextPlan.Id, out var planStat)
            ? planStat.PreviousWorkout.CompletedOn
            : (DateTimeOffset?)null;

        var summary = new NextWorkoutSummary
        {
            WorkoutPlanId = nextPlan.Id,
            WorkoutPlanName = nextPlan.Name,
            LastCompletedOn = lastCompleted,
            Exercises = nextPlan.PlannedExercises
                .OrderBy(e => e.Order)
                .ToImmutableArray()
        };

        await _localStorageContex.NextWorkoutSummary.SetAsync(summary);
    }
}
