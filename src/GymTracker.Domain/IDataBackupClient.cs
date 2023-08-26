namespace GymTracker.Domain;
public interface IDataBackupClient
{
    Task BackupAsync(string key, string dataAsString);
    Task<string> DownloadBackupItem(string key);
}
