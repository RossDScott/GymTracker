using Fluxor;
using GymTracker.Domain.LocalStorage;
using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public class ExercisesEffects
{
    private readonly GymTrackerLocalStorageContext _localStorageContext;

    public ExercisesEffects(GymTrackerLocalStorageContext localStorageContext)
	{
        _localStorageContext = localStorageContext;
    }

    [EffectMethod]
    public async Task OnFetchExercises(FetchExercisesAction action, IDispatcher dispatcher)
    {
        var exercises = await _localStorageContext.Exercises.GetOrDefaultAsync();
        dispatcher.Dispatch(new SetExercisesAction(exercises));
    }

    [EffectMethod]
    public async Task OnFetchExercise(FetchExerciseAction action, IDispatcher dispatcher)
    {
        var exercises = await _localStorageContext.Exercises.GetOrDefaultAsync();
        var exercise = exercises.Single(x => x.Id == action.Id);
        dispatcher.Dispatch(new SetExerciseAction(exercise));
    }

    [EffectMethod]
    public async Task OnAddOrUpdateExercise(AddOrUpdateExerciseAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.Exercise;
        var exercises = await _localStorageContext.Exercises.GetOrDefaultAsync();
        var exercise = exercises.SingleOrDefault(x => x.Id == action.Exercise.Id);

        var isNew = false;
        if(exercise is null)
        {
            exercise = new Exercise { Id = updateDTO.Id };
            exercises.Add(exercise);
            isNew = true;
        }
        
        exercise.Name = updateDTO.Name;
        exercise.MetricType = updateDTO.MetricType;
        exercise.BodyTarget = updateDTO.BodyTarget.ToArray();
        exercise.IsAcitve = updateDTO.IsActive;

        await _localStorageContext.Exercises.SetAsync(exercises);
        dispatcher.Dispatch(new SetExercisesAction(exercises));

        if(isNew)
            dispatcher.Dispatch(new NavigateToNewExerciseAction(exercise.Id));
    }
}
