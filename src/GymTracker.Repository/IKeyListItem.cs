namespace GymTracker.Repository;

public interface IKeyListItem<T> : IKeyItem<ICollection<T>>
{
    void ConfigureList(Action<KeyListItemConfig<ICollection<T>, T>> configureSettings);
    Task<T> FindByIdAsync(Guid id);
    Task<T?> FindOrDefaultByIdAsync(Guid id);
    Task<UpsertResponse> UpsertAsync(T item);
}

public enum UpsertResponse
{
    Existing,
    New
}