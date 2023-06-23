namespace GymTracker.Domain.Models;
public record WorkoutPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<WorkoutExercise> PlannedExercises { get; set; } = new List<WorkoutExercise>();
}

public record WorkoutExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Exercise Exercise { get; set; } = default!;
}

