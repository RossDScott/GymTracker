namespace GymTracker.Domain.Models.Extensions;
public static class ExerciseSetMetricsExtensions
{
    public static ExerciseSetMetrics? GetMaxSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metrics.MaxBy(x => x.ToStandardMeasure(metricType));

    public static ExerciseSetMetrics? GetMinSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metrics.MinBy(x => x.ToStandardMeasure(metricType));

    public static ExerciseSetMetrics? GetMaxVolumeSet(this IEnumerable<ExerciseSetMetrics> metrics, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => metrics.MaxBy(x => (x.Weight ?? 0) * (x.Reps ?? 0)),
            MetricType.Time => metrics.MaxBy(x => x.Time ?? 0),
            MetricType.TimeAndDistance => metrics.MaxBy(x => x.Distance ?? 0),
            MetricType.Reps => metrics.MaxBy(x => x.Reps ?? 0),
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static decimal ToStandardMeasure(this ExerciseSetMetrics metrics, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => metrics.Weight ?? 0,
            MetricType.Time => metrics.Time ?? 0,
            MetricType.TimeAndDistance => metrics.Distance ?? 0,
            MetricType.Reps => metrics.Reps ?? 0,
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static string GetMeasureText(this ExerciseSetMetrics set, MetricType metricType)
        => set.ToFormattedMetric(metricType);

    public static string GetWeightTotalVolumeWithMeasure(this IEnumerable<ExerciseSetMetrics> sets)
        => $"{sets.Sum(x => x.Weight * x.Reps)} Kg";

    public static string GetTotalVolumeWithMeasure(this IEnumerable<ExerciseSetMetrics> sets, MetricType metricType)
        => GetTotalVolume(sets, metricType)
            .ToString()
            .WithFormattedMetricMeasureMetric(metricType);

    public static decimal GetTotalVolume(this IEnumerable<ExerciseSetMetrics> sets, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => sets.Sum(x => (x.Weight ?? 0) * (x.Reps ?? 0)),
            MetricType.Time => sets.Sum(x => x.Time ?? 0),
            MetricType.Reps => sets.Sum(x => x.Reps ?? 0),
            _ => throw new ArgumentOutOfRangeException(nameof(metricType))
        };

    public static string ToFormattedMetric(this ExerciseSetMetrics set, MetricType metricType)
        => (metricType switch
        {
            MetricType.Weight => $"{set.Reps ?? 0} x {set.Weight ?? 0}",
            MetricType.Time => set.Time?.ToString() ?? "0",
            MetricType.TimeAndDistance => set.Distance?.ToString() ?? "0",
            MetricType.Reps => set.Reps?.ToString() ?? "0",
            _ => throw new ArgumentException()
        }).WithFormattedMetricMeasureMetric(metricType);
}
