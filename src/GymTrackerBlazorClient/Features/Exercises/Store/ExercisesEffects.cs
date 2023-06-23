using Fluxor;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public class ExercisesEffects
{
    private readonly IClientStorage _clientStorage;

    public ExercisesEffects(IClientStorage clientStorage)
	{
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchExercises(FetchExercisesAction action, IDispatcher dispatcher)
    {
        var settings = await _clientStorage.AppSettings.GetOrDefaultAsync();
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();
        
        dispatcher.Dispatch(new SetInitialDataAction(
            action.SelectedId,
            settings.TargetBodyParts,
            settings.Equipment,
            exercises));
    }

    [EffectMethod]
    public async Task OnFetchExercise(FetchExerciseAction action, IDispatcher dispatcher)
    {
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();
        var exercise = exercises.Single(x => x.Id == action.Id);
        dispatcher.Dispatch(new SetExerciseAction(exercise));
    }

    [EffectMethod]
    public async Task OnAddOrUpdateExercise(AddOrUpdateExerciseAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.Exercise;
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();
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
        exercise.Equipment = updateDTO.Equipment.ToArray();
        exercise.IsAcitve = updateDTO.IsActive;

        await _clientStorage.Exercises.SetAsync(exercises);
        dispatcher.Dispatch(new SetExercisesAction(exercises));

        if(isNew)
            dispatcher.Dispatch(new NavigateToNewExerciseAction(exercise.Id));
    }
}
