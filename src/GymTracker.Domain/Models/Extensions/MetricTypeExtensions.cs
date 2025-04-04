﻿namespace GymTracker.Domain.Models.Extensions;
public static class MetricTypeExtensions
{
    public static string WithFormattedMetricMeasureMetric(this string text, MetricType metricType)
        => text + " " + metricType.ToMetricDescription();

    public static string ToMetricDescription(this MetricType metricType)
        => metricType switch
        {
            MetricType.Weight => $"Kg",
            MetricType.Time => $"seconds",
            MetricType.TimeAndDistance => "seconds",
            MetricType.Reps => "reps",
            _ => throw new ArgumentException()
        };
}