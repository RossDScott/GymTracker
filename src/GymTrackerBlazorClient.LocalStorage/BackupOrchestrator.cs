using GymTracker.Domain;
using GymTracker.Repository;

namespace GymTracker.LocalStorage;
public class BackupOrchestrator : IBackupOrchestrator
{
    private readonly ClientStorageContext _clientStorage;
    private readonly IDataBackupClient _dataBackupClient;

    private readonly List<IKeyItem> _keyItemsToBackup;

    private bool _isRestoring = false;

    public BackupOrchestrator(ClientStorageContext clientStorage, IDataBackupClient dataBackupClient)
    {
        _clientStorage = clientStorage;
        _dataBackupClient = dataBackupClient;

        _keyItemsToBackup = _clientStorage.Keys
            .Where(x => x.AutoBackup)
            .ToList();

        SetupAutoBackup();
    }

    public async Task Backup()
    {
        foreach (var keyItem in _keyItemsToBackup)
        {
            var json = await keyItem.DataAsJson();
            if (string.IsNullOrWhiteSpace(json))
                continue;

            await _dataBackupClient.BackupAsync(keyItem.KeyName, json);
        }
    }

    public async Task Restore()
    {
        _isRestoring = true;
        try
        {
            foreach (var keyItem in _keyItemsToBackup)
            {
                var json = await _dataBackupClient.DownloadBackupItem(keyItem.KeyName);
                if (string.IsNullOrWhiteSpace(json))
                    continue;

                await keyItem.SetDataFromJson(json);
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _isRestoring = false;
        }
    }

    private void SetupAutoBackup()
    {
        foreach (var keyItem in _keyItemsToBackup)
        {
            keyItem.SubscribeToChangesAsJson(async dataAsJson =>
            {
                if (_isRestoring)
                    return;

                await _dataBackupClient.BackupAsync(keyItem.KeyName, dataAsJson);
            });
        }
    }
}
