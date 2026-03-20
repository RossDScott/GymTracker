namespace GymTracker.LocalStorage.Core;

public interface IKeyListItem<T> : IKeyItem<ICollection<T>>
{
    void ConfigureList(Action<KeyListItemConfig<ICollection<T>, T>> configureSettings);
    Task<T> FindByIdAsync(Guid id);
    Task<T?> FindOrDefaultByIdAsync(Guid id);
    Task UpsertAsync(T item);
    void SubscribeToItemUpsert(Func<T, Task> callback);
}