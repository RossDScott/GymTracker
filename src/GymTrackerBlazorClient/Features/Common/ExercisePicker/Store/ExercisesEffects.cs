using Fluxor;
using GymTracker.BlazorClient.Shared;
using GymTracker.LocalStorage.Core;

namespace GymTracker.BlazorClient.Features.ExercisePicker.Store;

public class ExercisePickerEffects : EffectsBase
{
    private readonly IClientStorage _clientStorage;

    public ExercisePickerEffects(IClientStorage clientStorage, ErrorService errorService)
        : base(errorService)
	{
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchExercises(FetchExercisesAction action, IDispatcher dispatcher) =>
        await SafeHandle(async () =>
        {
            var settings = await _clientStorage.AppSettings.GetOrDefaultAsync();
            var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();

            dispatcher.Dispatch(new SetInitialDataAction(
                settings.TargetBodyParts,
                settings.Equipment,
                exercises));
        });
}
