namespace GymTracker.Domain.Models.Extensions;
public static class PlannedExerciseExtensions
{
    public static ExerciseSetMetrics? GetMaxSet(this PlannedExercise plannedExercise)
        => plannedExercise.PlannedSets
            .Select(x => x.TargetMetrics)
            .GetMaxSet(plannedExercise.Exercise.MetricType);
}
