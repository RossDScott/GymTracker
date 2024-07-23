namespace GymTracker.Domain.Models;
public record WorkoutPlan
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<PlannedExercise> PlannedExercises { get; set; } = new List<PlannedExercise>();

    public bool IsAcitve { get; set; } = true;
}

public record PlannedExercise : IOrderable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Order { get; set; }
    public required Exercise Exercise { get; set; }
    public ICollection<PlannedExerciseSet> PlannedSets { get; set; } = new List<PlannedExerciseSet>();
    public TimeSpan RestInterval { get; set; }
    public bool AutoTriggerRestTimer { get; set; } = true;
    public int TargetRepsLower { get; set; }
    public int TargetRepsUpper { get; set; }
    public decimal TargetWeightIncrement { get; set; }
}

public record PlannedExerciseSet
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Order { get; set; }
    public required string SetType { get; set; }
    public int OrderForSetType { get; set; }
    public required ExerciseSetMetrics TargetMetrics { get; set; }
}