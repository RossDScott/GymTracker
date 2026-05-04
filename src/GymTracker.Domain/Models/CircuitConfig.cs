namespace GymTracker.Domain.Models;

public record CircuitConfig
{
    public int Rounds { get; set; } = 3;
    public TimeSpan RestBetweenRounds { get; set; } = TimeSpan.FromMinutes(2);
}
