namespace GymTracker.Domain.Models.Extensions;
public static class WorkoutExerciseExtensions
{
    public static ExerciseSetMetrics? GetMaxSet(this WorkoutExercise workoutExercise)
        => workoutExercise.Sets
            .Select(x => x.Metrics)
            .GetMaxSet(workoutExercise.Exercise.MetricType);
}