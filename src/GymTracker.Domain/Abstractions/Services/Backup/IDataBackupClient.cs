namespace GymTracker.Domain.Abstractions.Services.Backup;
public interface IDataBackupClient
{
    Task BackupAsync(string dataAsString, string key);
    Task<string> DownloadBackupItem(string key);
}
