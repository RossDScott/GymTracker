namespace GymTracker.LocalStorage.Core;
public interface IDefaultDataSource<T>
{
    Task<T?> LoadData();
}
