using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;

namespace GymTracker.LocalStorage.Triggers;
internal class ExerciseUpdateTrigger : ITrigger
{
    private readonly ClientStorageContext _localStorageContex;

    public ExerciseUpdateTrigger(ClientStorageContext localStorageContex)
    {
        _localStorageContex = localStorageContex;
    }

    public void Subscribe()
    {
        _localStorageContex.Exercises.SubscribeToChanges(ExercisesChanged);
    }

    public async Task ExercisesChanged(ICollection<Exercise> exercises)
    {
        var plans = await _localStorageContex.WorkoutPlans.GetOrDefaultAsync();

        foreach (var exercise in exercises)
        {
            foreach (var plan in plans)
            {
                var mactchingExercises = plan.PlannedExercises
                                                .Where(x => x.Exercise.Id == exercise.Id);

                foreach (var matchingExercise in mactchingExercises)
                {
                    matchingExercise.Exercise = exercise;
                }
            }
        }

        await _localStorageContex.WorkoutPlans.SetAsync(plans);
    }
}
