namespace GymTrackerBlazorPOC.Session;

public class WorkoutPlan
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IEnumerable<IExercise<SetMetrics>>? Exercises { get; set; }
}
public interface SetMetrics { }

public enum MetricType
{
    Weight,
    Time
}

public interface IExercise<T> where T : SetMetrics
{
    string Name { get; set; }
    IEnumerable<IExerciseTargetSet<T>> TargetSets { get; set; }
}

public class Exercise<T> : IExercise<T> where T : SetMetrics
{
    public string Name { get; set; }
    public IEnumerable<IExerciseTargetSet<T>>? TargetSets { get; set; }
}

public interface IExerciseTargetSet<T> where T : SetMetrics
{
    string SetType { get; set; }
    T Target { get; set; }
}

public class ExerciseTargetSet<T> : IExerciseTargetSet<T> where T : SetMetrics
{
    public string? SetType { get; set; }
    public T? Target { get; set; }
}

public class SetWeightMetrics : SetMetrics
{
    public decimal Weight { get; set; }
    public int Reps { get; set; }
}

public class SetTimeMetrics : SetMetrics
{
    public decimal TimeMilliSeconds { get; set; }
}

public class ExerciseSetVM<T> where T : SetMetrics
{
    public string? SetType { get; set; }
    public T? TargetMetrics { get; set; }
    public T? ActualMetrics { get; set; }
}

public class ExerciseVM<T> where T : SetMetrics
{
    public IExercise<T>? Exercise { get; set; }
    public IEnumerable<ExerciseSetVM<T>>? Sets { get; set; }
}

public class SessionVM
{
    public DateTime WorkoutStart { get; set; }
    public WorkoutPlan? WorkoutPlan { get; set; }

    public IEnumerable<ExerciseVM<SetMetrics>>? Exercises { get; set; }
}
