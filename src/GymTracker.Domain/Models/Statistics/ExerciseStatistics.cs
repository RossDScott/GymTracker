namespace GymTracker.Domain.Models.Statistics;
public record ExerciseStatistic
{
    public Guid ExerciseId { get; init; }
    public string ExerciseName { get; set; } = "";
    public required ICollection<ExerciseLog> Logs { get; init; }
    public bool ShowChartOnHomePage { get; init; } = false;
    public string ExerciseMetric { get; init; } = "";
}

public record ExerciseLog
{
    public DateTimeOffset WorkoutDateTime { get; init; }

    public required ICollection<ExerciseSetMetrics> Sets { get; init; }
    public decimal TotalVolume { get; init; } = 0;
}