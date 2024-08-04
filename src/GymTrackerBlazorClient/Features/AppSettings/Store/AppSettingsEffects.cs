using Fluxor;
using GymTracker.Domain;
using GymTracker.LocalStorage;
using MudBlazor;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public class AppSettingsEffects
{
    private readonly ClientStorageContext _clientStorage;
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackupOrchestrator _backupOrchestrator;
    private readonly ISnackbar _snackbar;

    public AppSettingsEffects(
        ClientStorageContext clientStorage,
        IServiceProvider serviceProvider,
        IBackupOrchestrator backupOrchestrator,
        ISnackbar snackbar)
    {
        _clientStorage = clientStorage;
        _serviceProvider = serviceProvider;
        _backupOrchestrator = backupOrchestrator;
        _snackbar = snackbar;
    }

    [EffectMethod]
    public async Task OnFetchSettings(FetchSettingsAction action, IDispatcher dispatcher)
    {
        var settings = await GetSettings();
        dispatcher.Dispatch(new SetSettingsAction(settings));
    }

    [EffectMethod]
    public async Task OnSetAzureBlobBackupContainerSASURI
        (UpdateAzureBlobBackupContainerSASURIAction action, IDispatcher dispatcher)
    {
        var current = await GetSettings();
        var updated = current with { AzureBlobBackupContainerSASURI = action.URI };
        await _clientStorage.AppSettings.SetAsync(updated);
        _snackbar.Add("Backup SAS URI Updated", Severity.Success);
    }

    [EffectMethod]
    public async Task OnBackupAll(BackupAllAction action, IDispatcher dispatcher)
    {
        await _backupOrchestrator.Backup();
        _snackbar.Add("Backup complete", Severity.Success);
    }

    [EffectMethod]
    public async Task OnRestoreAll(RestoreAllAction action, IDispatcher dispatcher)
    {
        await _backupOrchestrator.Restore();
        _snackbar.Add("Restore backup complete", Severity.Success);
    }

    [EffectMethod]
    public async Task OnDeleteAll(DeleteAllAction action, IDispatcher dispatcher)
    {
        await _clientStorage.DeleteAll();
    }

    [EffectMethod]
    public async Task OnTargetBodyPartsChange(UpdateTargetBodyAction action, IDispatcher dispatcher) =>
        await UpdateSettings(settings =>
            settings with { TargetBodyParts = action.TargetBodyParts }, dispatcher);

    [EffectMethod]
    public async Task OnExercisesChange(UpdateEquipmentAction action, IDispatcher dispatcher) =>
    await UpdateSettings(settings =>
        settings with { Equipment = action.Equipment }, dispatcher);

    [EffectMethod]
    public async Task OnSetTypesChange(UpdateSetTypesAction action, IDispatcher dispatcher) =>
    await UpdateSettings(settings =>
        settings with { SetType = action.SetTypes }, dispatcher);

    private async Task UpdateSettings(
        Func<Models.AppSettings, Models.AppSettings> updateFunc,
        IDispatcher dispatcher)
    {
        var existing = await GetSettings();
        var updated = updateFunc(existing);
        await _clientStorage.AppSettings.SetAsync(updated);
        dispatcher.Dispatch(new SetSettingsAction(updated));
    }

    private ValueTask<Models.AppSettings> GetSettings() =>
        _clientStorage.AppSettings.GetOrDefaultAsync();
}
