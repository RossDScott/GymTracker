using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.Models;


public record WorkoutPlan
{
    public Guid Id { get; init; }
    public string Name { get; set; } = default!;

    public IList<WorkoutTargetExercise> PlannedExercises { get; set; } = new List<WorkoutTargetExercise>();
}

public record WorkoutTargetExercise
{
    public Guid Id { get; init; }
    public Exercise Exercise { get; set; } = default!;

    public IList<ExerciseSet> TargetSets { get; set; } = new List<ExerciseSet>();
}

public enum MetricType
{
    Weight,
    Time
}

public record Exercise
{
    public Guid Id { get; init; }
    public string Name { get; set; } = default!;
    public MetricType MetricType { get; set; }
    public string Units { get; set; } = "Kg";
}

public record ExerciseSet
{
    public Guid Id { get; init; }
    public string Name { get; set; } = default!;
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Time { get; set; }
}

public record Session
{
    public Session(WorkoutPlan workoutPlan)
    {
        this.WorkoutPlan = workoutPlan;
    }

    public Guid Id { get; init; }
    public DateTimeOffset WorkoutStart { get; set; }
    public WorkoutPlan WorkoutPlan { get; }

    public IList<SessionExercise> Exercises { get; set; } = new List<SessionExercise>();
}

public record SessionExercise
{
    public SessionExercise(Exercise exercise)
    {
        this.Exercise = exercise;
    }

    public Guid Id { get; init; }
    public Exercise Exercise { get; }
    public IList<SessionExerciseSet> Sets { get; set; } = new List<SessionExerciseSet>();
}

public record SessionExerciseSet
{
    public SessionExerciseSet() : this(new ExerciseSet()){ }

    public SessionExerciseSet(ExerciseSet targetSet)
    {
        this.TargetSet = targetSet;
        this.ActualSet = new ExerciseSet { Name = targetSet.Name };
    }

    public Guid Id { get; init; }

    public ExerciseSet TargetSet { get; set; }
    public ExerciseSet ActualSet { get; set; }
}
