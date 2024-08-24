namespace GymTracker.Domain.Models;
public enum MetricType
{
    Weight,
    Time,
    TimeAndDistance
}

public record Exercise
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public MetricType MetricType { get; set; }
    public string[] BodyTarget { get; set; } = Array.Empty<string>();
    public string[] Equipment { get; set; } = Array.Empty<string>();
    public TimeSpan DefaultRestInterval { get; set; } = DefaultData.DefaultRestInterval;
    public bool IsAcitve { get; set; } = true;
    public bool ShowChartOnHomepage { get; set; } = false;
}