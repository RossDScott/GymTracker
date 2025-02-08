using Fluxor;

namespace GymTracker.BlazorClient.Features.Common;

[FeatureState]
public record SavingState
{
    public bool IsSaving { get; init; } = false;
}

public record SetIsSavingAction(bool IsSaving);

public static class SavingStateReducer
{
    [ReducerMethod]
    public static SavingState OnSetIsSaving(SavingState state, SetIsSavingAction action)
        => state with { IsSaving = action.IsSaving };
}