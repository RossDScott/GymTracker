namespace GymTracker.Domain.Models.Extensions;
public static class ExerciseSetMetricsExtensions
{
    public static ExerciseSetMetrics? GetMaxSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metrics.MaxBy(x => x.GetMeasure(metricType));

    public static ExerciseSetMetrics? GetMaxVolumeSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => metrics.MaxBy(x => (x.Weight ?? 0) * (x.Reps ?? 0)),
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static decimal GetMeasure(this ExerciseSetMetrics metrics, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => metrics.Weight ?? 0,
            MetricType.Time => metrics.Time ?? 0,
            MetricType.TimeAndDistance => metrics.Distance ?? 0,
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };
}
