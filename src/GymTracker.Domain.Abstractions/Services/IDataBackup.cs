namespace GymTracker.Domain.Abstractions.Services;
public interface IDataBackup
{
    Task BackupAsync(object data, string name);
    Task RestoreAllAsync();
}
