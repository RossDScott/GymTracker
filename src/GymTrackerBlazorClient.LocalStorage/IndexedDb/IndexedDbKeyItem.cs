using GymTracker.LocalStorage.Core;
using System.Text.Json;

namespace GymTracker.LocalStorage.IndexedDb;

internal record SingletonWrapper<T>
{
    public string Key { get; init; } = "singleton";
    public T? Value { get; init; }
}

public class IndexedDbKeyItem<T> : IKeyItem<T>
{
    protected readonly IIndexedDbService _db;
    protected KeyConfig<T> Config = new();

    private T? _cacheData = default;
    private bool _cacheLoaded = false;

    private readonly List<Func<T, Task>> _changeCallbacks = new();
    private readonly List<Action<string>> _jsonChangeCallbacks = new();

    public IndexedDbKeyItem(IIndexedDbService db, string key)
    {
        _db = db;
        Configure(settings => settings.Key = key);
    }

    public string KeyName => Config.Key;
    public bool AutoBackup => Config.AutoBackup;

    public void Configure(Action<KeyConfig<T>> configure) => configure(Config);

    public async ValueTask<T?> GetAsync()
    {
        if (Config.CacheData && _cacheLoaded)
            return _cacheData;

        var wrapper = await _db.GetAsync<SingletonWrapper<T>>(Config.Key, "singleton");
        var data = wrapper is not null ? wrapper.Value : default;

        if (Config.CacheData)
        {
            _cacheData = data;
            _cacheLoaded = true;
        }

        return data;
    }

    public ValueTask<T> GetOrDefaultAsync() => GetOrDefaultAsync(null);

    public async ValueTask<T> GetOrDefaultAsync(Func<T>? defaultConstructor)
    {
        var constructor =
            defaultConstructor ??
            Config?.DefaultConstructor ??
            throw new ArgumentNullException(nameof(defaultConstructor));

        return await GetAsync() ?? constructor();
    }

    public async ValueTask SetAsync(T item)
    {
        if (Config.CacheData)
        {
            _cacheData = item;
            _cacheLoaded = true;
        }

        var wrapper = new SingletonWrapper<T> { Value = item };
        await _db.PutAsync(Config.Key, wrapper);

        await NotifyChangeCallbacks(item);
    }

    public async Task DeleteAsync()
    {
        await _db.DeleteAsync(Config.Key, "singleton");
        _cacheData = default;
        _cacheLoaded = false;
    }

    public void SubscribeToChanges(Func<T, Task> callback)
    {
        _changeCallbacks.Add(callback);
    }

    public void SubscribeToChangesAsJson(Action<string> callback)
    {
        _jsonChangeCallbacks.Add(callback);
    }

    public async ValueTask<string?> DataAsJson()
    {
        var data = await GetAsync();
        return data is null ? null : JsonSerializer.Serialize(data);
    }

    public async ValueTask SetDataFromJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return;

        var data = JsonSerializer.Deserialize<T>(jsonString);
        if (data is null)
            return;

        await SetAsync(data);
    }

    protected async Task NotifyChangeCallbacks(T data)
    {
        foreach (var callback in _changeCallbacks)
            await callback(data);

        if (_jsonChangeCallbacks.Count > 0)
        {
            var json = JsonSerializer.Serialize(data);
            if (!string.IsNullOrEmpty(json))
            {
                foreach (var callback in _jsonChangeCallbacks)
                    callback(json);
            }
        }
    }
}
