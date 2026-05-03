namespace GymTracker.Domain;

public interface IBackupOrchestrator
{
    Task Backup();
    Task Restore();
    Task DeleteBlobAsync(string key);
    Task SyncFromBlobAsync();
}