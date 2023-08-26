namespace GymTracker.Domain;

public interface IBackupOrchestrator
{
    Task Backup();
    Task Restore();
}