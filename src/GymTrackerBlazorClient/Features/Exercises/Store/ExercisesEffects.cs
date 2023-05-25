using Fluxor;
using GymTracker.Domain.LocalStorage;

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
    public async Task OnUpdateExercise(UpdateExerciseAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.Exercise;
        var exercises = await _localStorageContext.Exercises.GetOrDefaultAsync();
        var exercise = exercises.Single(x => x.Id == action.Exercise.Id);
        
        exercise.Name = updateDTO.Name;
        exercise.MetricType = updateDTO.MetricType;
        exercise.BodyTarget = updateDTO.BodyTarget.ToArray();

        await _localStorageContext.Exercises.SetAsync(exercises);
        dispatcher.Dispatch(new SetExercisesAction(exercises));
    }
}
