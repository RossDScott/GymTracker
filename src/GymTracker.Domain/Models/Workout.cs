using System.Text.Json.Serialization;

namespace GymTracker.Domain.Models;
public record Workout
{
    [JsonConstructor]
    public Workout(WorkoutPlan plan)
    {
        Plan = plan;
        Exercises = plan.PlannedExercises
            .Select(plannedExercise => new WorkoutExercise(plannedExercise))
            .ToList();
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public WorkoutPlan Plan { get; set; }

    public DateTimeOffset WorkoutStart { get; set; }
    public DateTimeOffset? WorkoutEnd { get; set; }

    public ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();

    public string Notes { get; set; } = string.Empty;
}

public record WorkoutExercise : IOrderable
{
    public WorkoutExercise(Exercise exercise)
    {
        Exercise = exercise;
    }

    [JsonConstructor]
    public WorkoutExercise(PlannedExercise plannedExercise)
    {
        Exercise = plannedExercise.Exercise;
        PlannedExercise = plannedExercise;

        Sets = plannedExercise.PlannedSets
            .Select(plannedSet => new WorkoutExerciseSet(plannedSet))
            .ToList();
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public int Order { get; set; }
    public Exercise Exercise { get; set; }
    public PlannedExercise? PlannedExercise { get; set; } = null;

    public ICollection<WorkoutExerciseSet> Sets { get; set; } = new List<WorkoutExerciseSet>();
}

public record WorkoutExerciseSet
{
    [JsonConstructor]
    public WorkoutExerciseSet(PlannedExerciseSet? plannedExerciseSet)
    {
        PlannedExerciseSet = plannedExerciseSet;

        if (plannedExerciseSet != null)
        {
            Order = plannedExerciseSet.Order;
            SetType = plannedExerciseSet.SetType;
            OrderForSetType = plannedExerciseSet.OrderForSetType;
        }
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public PlannedExerciseSet? PlannedExerciseSet { get; set; } = null;
    public int Order { get; set; }
    public string SetType { get; set; } = default!;
    public int OrderForSetType { get; set; }
    public ExerciseSetMetrics Metrics { get; set; } = new ExerciseSetMetrics();
    public bool Completed { get; set; } = false;
    public DateTimeOffset? CompletedOn { get; set; } = null;
}