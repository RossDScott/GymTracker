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
        _localStorageContex.Exercises.SubscribeToItemUpsert(exercise =>
        {
            _ = ExerciseUpserted(exercise);
            return Task.CompletedTask;
        });
    }

    private async Task ExerciseUpserted(Exercise exercise)
    {
        var plans = await _localStorageContex.WorkoutPlans.GetOrDefaultAsync();

        foreach (var plan in plans)
        {
            var matchingExercises = plan.PlannedExercises
                                        .Where(x => x.Exercise.Id == exercise.Id)
                                        .ToList();

            if (!matchingExercises.Any())
                continue;

            foreach (var matchingExercise in matchingExercises)
            {
                matchingExercise.Exercise = exercise;
            }

            await _localStorageContex.WorkoutPlans.UpsertAsync(plan);
        }
    }
}
