using Fluxor;
using GymTracker.LocalStorage.Core;

namespace GymTracker.BlazorClient.Features.ExercisePicker.Store;

public class ExercisePickerEffects
{
    private readonly IClientStorage _clientStorage;

    public ExercisePickerEffects(IClientStorage clientStorage)
	{
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchExercises(FetchExercisesAction action, IDispatcher dispatcher)
    {
        var settings = await _clientStorage.AppSettings.GetOrDefaultAsync();
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();
        
        dispatcher.Dispatch(new SetInitialDataAction(
            settings.TargetBodyParts,
            settings.Equipment,
            exercises));
    }
}
