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
    public string Name { get; set; } = default!;
    public MetricType MetricType { get; set; }
    public string[] BodyTarget { get; set; } = Array.Empty<string>();
    public string[] Equipment { get; set; } = Array.Empty<string>();
    public TimeSpan DefaultRestInterval { get; set; } = TimeSpan.FromMinutes(2);
    public bool IsAcitve { get; set; } = true;
}