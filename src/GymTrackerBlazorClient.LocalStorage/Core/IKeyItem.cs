namespace GymTracker.LocalStorage.Core;

public interface IKeyItem
{
    bool AutoBackup { get; }

    string KeyName { get; }
    ValueTask<string?> DataAsJson();
    ValueTask SetDataFromJson(string jsonString);
    void SubscribeToChangesAsJson(Action<string> callback);
    Task DeleteAsync();
}

public interface IKeyItem<T> : IKeyItem
{
    ValueTask<T?> GetAsync();
    ValueTask<T> GetOrDefaultAsync(Func<T> defaultConstructor);
    ValueTask<T> GetOrDefaultAsync();
    ValueTask SetAsync(T item);
    void SubscribeToChanges(Func<T, Task> callback);

    void Configure(Action<KeyConfig<T>> configureSettings);
}