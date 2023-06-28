namespace GymTracker.Domain.Models;
public record ExerciseSet
{
    public Guid Id { get; init; }
    public string SetType { get; set; } = default!;

}

public record ExerciseSetMetrics
{
    public int? Reps { get; init; }
    public decimal? Weight { get; set; }
    public decimal? Time { get; set; }
    public decimal? Distance { get; set; }
}