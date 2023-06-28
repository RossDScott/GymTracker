namespace GymTracker.Domain.Models;
public record WorkoutPlan
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<PlannedExercise> PlannedExercises { get; set; } = new List<PlannedExercise>();
}

public record PlannedExercise
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Exercise Exercise { get; set; } = default!;
}

public record PlannedExerciseSet
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string SetType { get; set; } = default!;
    public ExerciseSetMetrics TargetMetrics { get; set; } = default!;
}