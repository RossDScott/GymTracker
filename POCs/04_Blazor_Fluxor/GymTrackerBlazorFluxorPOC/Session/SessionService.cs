namespace GymTrackerBlazorFluxorPOC.Session;

public class SessionService
{
    //public SessionVM FetchSession()


    public List<Exercise<SetMetrics>> BuildExercises()
    {
        return new List<Exercise<SetMetrics>>
        {
            new Exercise<SetMetrics>{Id = 1, Name = "Incline Bench Press", TargetSets=null},
            new Exercise<SetMetrics>{Id = 2, Name = "Incline Dumbell Bench Press", TargetSets=null},
        };
    }

    public List<IExercise<SetMetrics>> SetupFakeExercises()
    {
        return new List<IExercise<SetMetrics>>
        {
            new Exercise<SetMetrics>{
                Name = "Barbell Bench Press",
                TargetSets = new List<ExerciseTargetSet<SetMetrics>>
                {
                    new ExerciseTargetSet<SetMetrics>
                    {
                        SetType = "Warm Up",
                        Target = new SetWeightMetrics{Weight = 20, Reps = 10}
                    }
                }
            },
            new Exercise<SetMetrics>{
                Name = "Dumbell Bench Press",
                TargetSets = new List<ExerciseTargetSet<SetMetrics>>
                {
                    new ExerciseTargetSet<SetMetrics>
                    {
                        SetType = "Warm Up",
                        Target = new SetWeightMetrics{Weight = 20, Reps = 10}
                    }
                }
            },
            new Exercise<SetMetrics>{
                Name = "Triceps Pushdown",
                TargetSets = new List<ExerciseTargetSet<SetMetrics>>
                {
                    new ExerciseTargetSet<SetMetrics>
                    {
                        SetType = "Warm Up",
                        Target = new SetWeightMetrics{Weight = 20, Reps = 10}
                    }
                }
            }
        };
    }

    public SessionVM? CurrentSession { get; private set; }

    public SessionVM SetupFakeSession()
    {
        var workoutPlan = new WorkoutPlan
        {
            Id = 1,
            Name = "Push Day",
            Exercises = SetupFakeExercises()
        };

        var session = new SessionVM
        {
            WorkoutPlan = workoutPlan,
            WorkoutStart = DateTime.Now,
            Exercises = workoutPlan.Exercises.Select(workoutExercise => new ExerciseVM<SetMetrics>
            {
                Id = Guid.NewGuid(),
                Exercise = workoutExercise,
                Sets = workoutExercise.TargetSets.Select(targetSet => new ExerciseSetVM<SetMetrics> { SetType = targetSet.SetType, TargetMetrics = targetSet.Target, ActualMetrics = null })
            }).ToList()
        };

        CurrentSession = session;

        return session;
    }

}
