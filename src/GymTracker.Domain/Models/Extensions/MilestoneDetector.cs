using GymTracker.Domain.Models.Statistics;

namespace GymTracker.Domain.Models.Extensions;

public static class MilestoneDetector
{
    /// <summary>
    /// Detects set-level milestones for a single completed set during a workout.
    /// Compares against all historical sets plus earlier completed sets in the current workout.
    /// </summary>
    public static IReadOnlyList<SetMilestone> DetectSetMilestones(
        ExerciseSetMetrics completedSet,
        MetricType metricType,
        IEnumerable<ExerciseSetMetrics> historicalSets,
        IEnumerable<ExerciseSetMetrics> previousWorkoutSets)
    {
        var milestones = new List<SetMilestone>();
        var allPriorSets = historicalSets.Concat(previousWorkoutSets).ToList();

        switch (metricType)
        {
            case MetricType.Weight:
                DetectWeightMilestones(completedSet, allPriorSets, milestones);
                break;
            case MetricType.Reps:
                DetectRepsMilestone(completedSet, allPriorSets, milestones);
                break;
            case MetricType.Time:
                DetectTimeMilestone(completedSet, allPriorSets, milestones);
                break;
            case MetricType.TimeAndDistance:
                DetectDistanceMilestone(completedSet, allPriorSets, milestones);
                break;
        }

        return milestones;
    }

    /// <summary>
    /// Detects all milestones for an exercise at end-of-workout.
    /// Compares only against historical data (not intra-workout).
    /// Deduplicates to report only the best set per milestone type.
    /// </summary>
    public static ExerciseMilestones DetectExerciseMilestones(
        Guid exerciseId,
        string exerciseName,
        MetricType metricType,
        IReadOnlyList<ExerciseSetMetrics> workoutCompletedSets,
        decimal workoutTotalVolume,
        ExerciseStatistic? historicalStat)
    {
        var historicalSets = historicalStat?.Logs
            .SelectMany(l => l.Sets)
            .ToList() ?? new List<ExerciseSetMetrics>();

        var bestMilestones = new Dictionary<MilestoneType, (SetMilestone Milestone, decimal Value)>();

        foreach (var set in workoutCompletedSets)
        {
            var setMilestones = DetectSetMilestones(set, metricType, historicalSets, Enumerable.Empty<ExerciseSetMetrics>());
            foreach (var milestone in setMilestones)
            {
                var value = GetMilestoneValue(set, milestone.Type, metricType);
                if (!bestMilestones.TryGetValue(milestone.Type, out var existing) || value > existing.Value)
                {
                    bestMilestones[milestone.Type] = (milestone, value);
                }
            }
        }

        SetMilestone? volumeMilestone = null;
        if (workoutCompletedSets.Count > 0)
        {
            var maxHistoricalVolume = historicalStat?.Logs
                .Select(l => l.TotalVolume)
                .DefaultIfEmpty(0)
                .Max() ?? 0;

            if (workoutTotalVolume > maxHistoricalVolume)
            {
                var volumeText = workoutTotalVolume
                    .ToString()
                    .WithFormattedMetricMeasureMetric(metricType);
                volumeMilestone = new SetMilestone(
                    MilestoneType.MaxVolume,
                    $"New Volume Record: {volumeText}");
            }
        }

        return new ExerciseMilestones(
            exerciseId,
            exerciseName,
            bestMilestones.Values.Select(v => v.Milestone).ToList(),
            volumeMilestone);
    }

    private static void DetectWeightMilestones(
        ExerciseSetMetrics completedSet,
        List<ExerciseSetMetrics> allPriorSets,
        List<SetMilestone> milestones)
    {
        var currentWeight = completedSet.Weight ?? 0;
        var currentReps = completedSet.Reps ?? 0;

        if (currentWeight <= 0) return;

        // Weight PR: heaviest weight ever lifted
        var maxPriorWeight = allPriorSets
            .Select(s => s.Weight ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        if (currentWeight > maxPriorWeight)
        {
            milestones.Add(new SetMilestone(
                MilestoneType.WeightPR,
                $"New Weight PR: {completedSet.ToFormattedMetric(MetricType.Weight)}"));
        }

        // Max reps at this weight
        if (currentReps > 0)
        {
            var maxRepsAtWeight = allPriorSets
                .Where(s => s.Weight == currentWeight)
                .Select(s => s.Reps ?? 0)
                .DefaultIfEmpty(0)
                .Max();

            if (currentReps > maxRepsAtWeight)
            {
                milestones.Add(new SetMilestone(
                    MilestoneType.MaxRepsAtWeight,
                    $"New Rep Record at {currentWeight} Kg: {currentReps} reps"));
            }
        }
    }

    private static void DetectRepsMilestone(
        ExerciseSetMetrics completedSet,
        List<ExerciseSetMetrics> allPriorSets,
        List<SetMilestone> milestones)
    {
        var currentReps = completedSet.Reps ?? 0;
        if (currentReps <= 0) return;

        var maxPriorReps = allPriorSets
            .Select(s => s.Reps ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        if (currentReps > maxPriorReps)
        {
            milestones.Add(new SetMilestone(
                MilestoneType.MaxReps,
                $"New Rep Record: {currentReps} reps"));
        }
    }

    private static void DetectTimeMilestone(
        ExerciseSetMetrics completedSet,
        List<ExerciseSetMetrics> allPriorSets,
        List<SetMilestone> milestones)
    {
        var currentTime = completedSet.Time ?? 0;
        if (currentTime <= 0) return;

        var maxPriorTime = allPriorSets
            .Select(s => s.Time ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        if (currentTime > maxPriorTime)
        {
            milestones.Add(new SetMilestone(
                MilestoneType.MaxTime,
                $"New Time Record: {currentTime} seconds"));
        }
    }

    private static void DetectDistanceMilestone(
        ExerciseSetMetrics completedSet,
        List<ExerciseSetMetrics> allPriorSets,
        List<SetMilestone> milestones)
    {
        var currentDistance = completedSet.Distance ?? 0;
        if (currentDistance <= 0) return;

        var maxPriorDistance = allPriorSets
            .Select(s => s.Distance ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        if (currentDistance > maxPriorDistance)
        {
            milestones.Add(new SetMilestone(
                MilestoneType.MaxDistance,
                $"New Distance Record: {currentDistance} km"));
        }
    }

    private static decimal GetMilestoneValue(ExerciseSetMetrics set, MilestoneType type, MetricType metricType)
        => type switch
        {
            MilestoneType.WeightPR => set.Weight ?? 0,
            MilestoneType.MaxRepsAtWeight => set.Reps ?? 0,
            MilestoneType.MaxReps => set.Reps ?? 0,
            MilestoneType.MaxTime => set.Time ?? 0,
            MilestoneType.MaxDistance => set.Distance ?? 0,
            _ => 0
        };
}
