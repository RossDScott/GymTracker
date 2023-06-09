using Fluxor;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Services;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public class AppSettingsEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackupOrchestrator _backupOrchestrator;
    private readonly ISnackbar _snackbar;

    public AppSettingsEffects(
        IClientStorage clientStorage, 
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
        var settings = await _clientStorage.AppSettings.GetOrDefaultAsync();
        var bodyTargets = await _clientStorage.TargetBodyParts.GetOrDefaultAsync();
        var equipment = await _clientStorage.Equipment.GetOrDefaultAsync();

        dispatcher.Dispatch(new SetSettingsAction(settings!, bodyTargets, equipment));
    }

    [EffectMethod]
    public async Task OnUpdateSettings(UpdateSettingsAction action, IDispatcher dispatcher)
    {
        await _clientStorage.AppSettings.SetAsync(action.Settings);
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
    public async Task OnAddTargetBody(AddTargetBodyAction action, IDispatcher dispatcher)
    {
        var existing = await _clientStorage.TargetBodyParts.GetOrDefaultAsync();
        existing.Add(action.TargetBody);
        await _clientStorage.TargetBodyParts.SetAsync(existing);
        dispatcher.Dispatch(new SetTargetBodyAction(existing));
    }

    [EffectMethod]
    public async Task OnDeleteTargetBody(DeleteTargetBodyAction action, IDispatcher dispatcher)
    {
        var existing = await _clientStorage.TargetBodyParts.GetOrDefaultAsync();
        existing.Remove(action.TargetBody);
        await _clientStorage.TargetBodyParts.SetAsync(existing);
        dispatcher.Dispatch(new SetTargetBodyAction(existing));
    }

    [EffectMethod]
    public async Task OnAddExercise(AddEquipmentAction action, IDispatcher dispatcher)
    {
        var existing = await _clientStorage.Equipment.GetOrDefaultAsync();
        existing.Add(action.equipment);
        await _clientStorage.Equipment.SetAsync(existing);
        dispatcher.Dispatch(new SetEquipmentAction(existing));
    }

    [EffectMethod]
    public async Task OnDeleteEquipment(DeleteEquipmentAction action, IDispatcher dispatcher)
    {
        var existing = await _clientStorage.Equipment.GetOrDefaultAsync();
        existing.Remove(action.equipment);
        await _clientStorage.Equipment.SetAsync(existing);
        dispatcher.Dispatch(new SetEquipmentAction(existing));
    }
}
