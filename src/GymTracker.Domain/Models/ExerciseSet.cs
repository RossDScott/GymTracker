namespace GymTracker.Domain.Models;
public record ExerciseSet
{
    public Guid Id { get; init; }
    public string SetType { get; set; } = default!;

}

