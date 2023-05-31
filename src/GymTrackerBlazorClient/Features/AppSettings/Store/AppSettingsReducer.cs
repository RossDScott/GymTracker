using Fluxor;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public static class AppSettingsReducer
{
    [ReducerMethod]
    public static AppSettingsState OnSetSettings(AppSettingsState state, SetSettingsAction action) =>
        state with { Settings = action.Settings };

    [ReducerMethod]
    public static AppSettingsState OnUpdateSettings(AppSettingsState state, UpdateSettingsAction action) =>
        state with { Settings = action.Settings };
}
