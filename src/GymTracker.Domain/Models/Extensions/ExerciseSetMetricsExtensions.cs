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

    public static string GetMeasureText(this ExerciseSetMetrics set, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => $"{set.Reps ?? 0} x {set.Weight ?? 0} Kg",
            MetricType.Time => $"{set.Time ?? 0} seconds",
            MetricType.TimeAndDistance => set.Distance?.ToString() ?? "",
            _ => ""
        };

    public static string GetMeasureTotalVolume(this IEnumerable<ExerciseSetMetrics> sets, MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => $"V: {sets.Select(x => x.Weight * x.Reps).Sum()} Kg",
            MetricType.Time => $"T: {sets.Select(x => x.Time).Sum()} seconds",
            MetricType.TimeAndDistance => $"D: {sets.Select(x => x.Distance).Sum()}",
            _ => ""
        };
}
