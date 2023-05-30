namespace GymTracker.Domain.Abstractions.Services;
public interface IDataBackupService
{
    Task BackupAsync(object? data, string name);
    Task RestoreAllAsync();
}
