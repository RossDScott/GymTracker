namespace GymTracker.Domain.Models;
public record ExerciseSetMetrics
{
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Time { get; set; }
    public decimal? Distance { get; set; }
}