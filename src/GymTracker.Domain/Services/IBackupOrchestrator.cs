namespace GymTracker.Domain.Services;

public interface IBackupOrchestrator
{
    Task Backup();
    Task Restore();
}