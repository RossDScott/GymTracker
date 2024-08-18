namespace GymTracker.Domain.Models.Extensions;
public static class ExerciseSetMetricsExtensions
{
    public static ExerciseSetMetrics? GetMaxSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metrics.MaxBy(x => x.ToStandardMeasure(metricType));

    public static ExerciseSetMetrics? GetMaxVolumeSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => metrics.MaxBy(x => (x.Weight ?? 0) * (x.Reps ?? 0)),
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static decimal ToStandardMeasure(this ExerciseSetMetrics metrics, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => metrics.Weight ?? 0,
            MetricType.Time => metrics.Time ?? 0,
            MetricType.TimeAndDistance => metrics.Distance ?? 0,
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static string GetMeasureText(this ExerciseSetMetrics set, MetricType metricType)
        => set.ToFormattedMetric(metricType);

    public static string GetWeightTotalVolumeWithMeasure(this IEnumerable<ExerciseSetMetrics> sets)
        => $"{sets.Select(x => x.Weight * x.Reps).Sum()} Kg";

    public static string GetTotalVolumeWithMeasure(this IEnumerable<ExerciseSetMetrics> sets, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => $"{sets.Select(x => x.Weight * x.Reps).Sum()} Kg",
            MetricType.Time => $"{sets.Select(x => x.Time ?? 0).Sum()} Seconds",
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static decimal GetWeightTotalVolume(this IEnumerable<ExerciseSetMetrics> sets)
        => sets.Select(x => (x.Weight ?? 0) * (x.Reps ?? 0)).Sum();

    public static string ToFormattedMetric(this ExerciseSetMetrics set, MetricType metricType)
        => (metricType switch
        {
            MetricType.Weight => $"{set.Reps ?? 0} x {set.Weight ?? 0}",
            MetricType.Time => $"{set.Time ?? 0}",
            MetricType.TimeAndDistance => set.Distance?.ToString() ?? "",
            _ => throw new ArgumentException()
        }).WithFormattedMetricMeasureMetric(metricType, set.ToStandardMeasure(metricType));
}
