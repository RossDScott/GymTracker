namespace GymTrackerBlazorPOC.Session;

public class SessionService
{
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
                Exercise = workoutExercise,
                Sets = workoutExercise.TargetSets.Select(targetSet => new ExerciseSetVM<SetMetrics> { SetType = targetSet.SetType, TargetMetrics = targetSet.Target, ActualMetrics = null })
            })
        };

        return session;
    }

}
