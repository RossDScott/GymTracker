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
    public PlannedExercise() {}

    public PlannedExercise(Exercise exercise)
    {
        Exercise = exercise;
        RestInterval = exercise.DefaultRestInterval;
    }
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Order { get; set; }
    public Exercise Exercise { get; set; } = default!;
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
    public string SetType { get; set; } = default!;
    public int OrderForSetType { get; set; }
    public ExerciseSetMetrics TargetMetrics { get; set; } = default!;
}