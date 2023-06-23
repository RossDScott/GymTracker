using Fluxor;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public static class AppSettingsReducer
{
    [ReducerMethod]
    public static AppSettingsState OnSetSettings(AppSettingsState state, SetSettingsAction action) =>
        state with 
        { 
            AzureBlobBackupContainerSASURI = action.Settings.AzureBlobBackupContainerSASURI,
            TargetBodyParts = action.Settings.TargetBodyParts.OrderBy(x => x).ToImmutableArray(),
            Equipment = action.Settings.Equipment.OrderBy(x => x).ToImmutableArray(),
            SetType = action.Settings.SetType
        };



    //[ReducerMethod]
    //public static AppSettingsState OnUpdateSettings(AppSettingsState state, UpdateSettingsAction action) =>
    //    state with { Settings = action.Settings };

    //[ReducerMethod]
    //public static AppSettingsState OnSetTargetBody(AppSettingsState state, SetTargetBodyAction action) =>
    //    state with { BodyTargets = action.TargetBodyParts.OrderBy(x => x).ToImmutableArray() };

    //[ReducerMethod]
    //public static AppSettingsState OnSetExercises(AppSettingsState state, SetEquipmentAction action) =>
    //    state with { Equipment = action.Equipment.OrderBy(x => x).ToImmutableArray() };
}
