using Fluxor;
using GymTracker.AzureBlobStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public class AppSettingsEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISnackbar _snackbar;

    public AppSettingsEffects(
        IClientStorage clientStorage, 
        IServiceProvider serviceProvider,
        ISnackbar snackbar)
    {
        _clientStorage = clientStorage;
        _serviceProvider = serviceProvider;
        _snackbar = snackbar;
    }

    [EffectMethod]
    public async Task OnFetchSettings(FetchSettingsAction action, IDispatcher dispatcher)
    {
        var settings = await _clientStorage.AppSettings.GetAsync();
        dispatcher.Dispatch(new SetSettingsAction(settings!));
    }

    [EffectMethod]
    public async Task OnUpdateSettings(UpdateSettingsAction action, IDispatcher dispatcher)
    {
        var blobBackupService = _serviceProvider.GetService<BlobBackupService>();
        if (blobBackupService != null)
            blobBackupService.Configure(new AzureBlobBackupSettings
            {
                ContainerSASURI = action.Settings.AzureBlobBackupContainerSASURI
            });

        await _clientStorage.AppSettings.SetAsync(action.Settings);
        _snackbar.Add("Backup SAS URI Updated", Severity.Success);
    }
        
}
