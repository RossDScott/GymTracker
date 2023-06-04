using GymTracker.Domain.Abstractions.Services.Backup;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.Domain.Services;
public class BackupOrchestrator : IBackupOrchestrator
{
    private readonly IClientStorage _clientStorage;
    private readonly IDataBackupClient _dataBackupClient;

    public BackupOrchestrator(IClientStorage clientStorage, IDataBackupClient dataBackupClient)
    {
        _clientStorage = clientStorage;
        _dataBackupClient = dataBackupClient;
    }

    public async Task Backup()
    {
        var keysToBackup = _clientStorage.Keys
            .Where(x => x.AutoBackup)
            .ToList();

        foreach (var keyItem in keysToBackup)
        {
            var json = await keyItem.DataAsJson();
            if (string.IsNullOrWhiteSpace(json))
                continue;

            await _dataBackupClient.BackupAsync(json, keyItem.KeyName);
        }
    }

    public async Task Restore()
    {
        var keysToRestore = _clientStorage.Keys
            .Where(x => x.AutoBackup)
            .ToList();

        foreach (var keyItem in keysToRestore)
        {
            var json = await _dataBackupClient.DownloadBackupItem(keyItem.KeyName);
            if (string.IsNullOrWhiteSpace(json))
                continue;

            await keyItem.SetDataFromJson(json);
        }
    }
}
