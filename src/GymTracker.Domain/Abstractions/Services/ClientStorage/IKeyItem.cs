using System.Collections;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;

public interface IKeyItem
{
    bool AutoBackup { get; }

    string KeyName { get; }
    ValueTask<string?> DataAsJson();
    ValueTask SetDataFromJson(string jsonString);
}

public interface IKeyItem<T> : IKeyItem
{
    ValueTask<T?> GetAsync();
    ValueTask<T> GetOrDefaultAsync(Func<T> defaultConstructor);
    ValueTask<T> GetOrDefaultAsync();
    ValueTask SetAsync(T item);
    void SubscribeToChanges(Action<T> callback);

    void Configure(Action<KeyConfig<T>> configureSettings);
}