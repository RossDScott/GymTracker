namespace GymTracker.Domain.Models;
public record ExerciseSet
{
    public Guid Id { get; init; }
    public string SetType { get; set; } = default!;
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Time { get; set; }
    public decimal? Distance { get; set; }
}
