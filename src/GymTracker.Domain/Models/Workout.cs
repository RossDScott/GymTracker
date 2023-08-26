using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Models;
public record Workout
{
    public Workout(WorkoutPlan plan)
    {
        Plan = plan;
    }

    public Guid Id { get; set; }
    public WorkoutPlan Plan { get; set; }

    public DateTimeOffset WorkoutStart { get; set; }
    public DateTimeOffset? WorkoutEnd { get; set; }


    public string Notes { get; set; } = string.Empty;


}

public record WorkoutExercise : IOrderable
{
    public WorkoutExercise(Exercise exercise)
    {
        Exercise = exercise;
    }

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
    public WorkoutExerciseSet(PlannedExerciseSet? targetSet)
    {
        PlannedExerciseSet = targetSet;
        var plannedMetrics = targetSet?.TargetMetrics;
        Metrics = plannedMetrics != null
            ? plannedMetrics with { }
            : new ExerciseSetMetrics();
    }

    public Guid Id { get; init; }
    public PlannedExerciseSet? PlannedExerciseSet { get; set; } = null;
    public int Order { get; set; }
    public string SetType { get; set; } = default!;
    public int OrderForSetType { get; set; }
    public ExerciseSetMetrics Metrics { get; set; }
}