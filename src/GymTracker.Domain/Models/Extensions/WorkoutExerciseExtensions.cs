using static GymTracker.Domain.Models.DefaultData;

namespace GymTracker.Domain.Models.Extensions;
public static class WorkoutExerciseExtensions
{
    public static ExerciseSetMetrics? GetMaxSet(this WorkoutExercise workoutExercise)
        => workoutExercise.Sets
            .Where(x => x.SetType == SetType.Set)
            .Select(x => x.Metrics)
            .GetMaxSet(workoutExercise.Exercise.MetricType);

    public static ExerciseSetMetrics? GetMinSet(this WorkoutExercise workoutExercise)
    => workoutExercise.Sets
        .Where(x => x.SetType == SetType.Set)
        .Select(x => x.Metrics)
        .GetMinSet(workoutExercise.Exercise.MetricType);
}