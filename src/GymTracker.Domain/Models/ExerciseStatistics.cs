namespace GymTracker.Domain.Models;
public record ExerciseStatistic
{
    public Guid ExerciseId { get; init; }

    public required ICollection<ExerciseLog> Logs { get; init; }
}

public record ExerciseLog
{
    public DateTimeOffset WorkoutDateTime { get; init; }

    public required ICollection<ExerciseSetMetrics> Sets { get; init; }
}