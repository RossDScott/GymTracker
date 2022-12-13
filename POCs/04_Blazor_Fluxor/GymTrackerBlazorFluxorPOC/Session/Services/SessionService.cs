using GymTrackerBlazorFluxorPOC.Session.Models;
using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.Services;

public class SessionService
{
    private List<Models.Exercise> setupFakeExercises() =>
        new List<Models.Exercise>
        {
            new Models.Exercise{Id = Guid.NewGuid(), Name = "Barbell Bench Press", MetricType = MetricType.Weight, Units = "Kg"},
            new Models.Exercise{Id = Guid.NewGuid(), Name = "Dumbell Bench Press", MetricType = MetricType.Weight, Units = "Kg"},
            new Models.Exercise{Id = Guid.NewGuid(), Name = "Triceps Pushdown", MetricType = MetricType.Weight, Units = "Kg"},
            new Models.Exercise{Id = Guid.NewGuid(), Name = "Plank", MetricType = MetricType.Time, Units = "Seconds"}
        };
    
    private IEnumerable<WorkoutTargetExercise> buildWorkoutTargetExercises()
    {
        var exercises = setupFakeExercises();

        return new List<WorkoutTargetExercise>
        {
            new WorkoutTargetExercise
            {
                Id = Guid.NewGuid(),
                Exercise = exercises[0],
                
                TargetSets = ImmutableList.Create<ExerciseSet>(
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Warm-up", Weight = 20, Reps = 10},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Warm-up", Weight = 20, Reps = 10},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 55, Reps = 8},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 55, Reps = 8},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 55, Reps = 8}
                    )
            },
            new WorkoutTargetExercise
            {
                Id = Guid.NewGuid(),
                Exercise = exercises[1],
                TargetSets = ImmutableList.Create<ExerciseSet>(
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 18, Reps = 10},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 18, Reps = 10},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 18, Reps = 10}
                    )
            },
            new WorkoutTargetExercise
            {
                Id = Guid.NewGuid(),
                Exercise = exercises[2],
                TargetSets = ImmutableList.Create<ExerciseSet>(
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 18, Reps = 10},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 18, Reps = 10},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Weight = 18, Reps = 10}
                    )
            },
            new WorkoutTargetExercise
            {
                Id = Guid.NewGuid(),
                Exercise = exercises[3],
                TargetSets = ImmutableList.Create<ExerciseSet>(
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Time = 60},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Time = 60},
                        new ExerciseSet{Id = Guid.NewGuid(),Name = "Set", Time = 60}
                    )
            },
        };
    }

    private WorkoutPlan setupFakeWorkoutPlan() => new WorkoutPlan
    {
        Id = Guid.NewGuid(),
        Name = "Push Day",
        PlannedExercises = buildWorkoutTargetExercises().ToImmutableList()
    };

    public Models.Session? CurrentSession { get; private set; }

    public WorkoutPlan FetchWorkoutPlan(Guid workoutId) => setupFakeWorkoutPlan();

    public Models.Session CreateNewSession(Guid workoutId)
    {
        var workout = FetchWorkoutPlan(workoutId);

        var session = new Models.Session(workout)
        {
            Id = Guid.NewGuid(),
            WorkoutStart = DateTimeOffset.Now,
            Exercises = workout.PlannedExercises.Select(exercise =>
                new SessionExercise(exercise.Exercise)
                {
                    Id = Guid.NewGuid(),
                    Sets = exercise.TargetSets.Select(ts => new SessionExerciseSet(ts) { Id = Guid.NewGuid() }).ToList()
                }).ToImmutableList()
        };

        this.CurrentSession = session;

        return session;
    }

    public Models.Session FetchExistingSession(Guid sessionId)
    {
        if(CurrentSession is null)
            CreateNewSession(Guid.NewGuid());

        return CurrentSession!;
    }

    //public SessionExercise FetchExercise(Guid id) => CurrentSession?.Exercises?.SingleOrDefault(x => x.Id == id);

}
