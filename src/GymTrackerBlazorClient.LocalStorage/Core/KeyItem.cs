using Blazored.LocalStorage;
using System.Text.Json;

namespace GymTracker.LocalStorage.Core;

public class KeyItem<T> : IKeyItem<T>
{
    protected readonly ILocalStorageService LocalStorage;
    //protected readonly string Key;
    protected KeyConfig<T> Config = new();

    private T? _cacheData = default;

    public KeyItem(ILocalStorageService localStorage, string key)
    {
        LocalStorage = localStorage;
        Configure(settings => settings.Key = key);
    }

    protected async ValueTask<T?> FetchDataOrCacheAsync()
    {
        var fetchData = () => LocalStorage.GetItemAsync<T?>(Config.Key);

        if (Config.CacheData)
        {
            if (_cacheData == null)
                _cacheData = await fetchData();

            return _cacheData;
        }

        return await fetchData();
    }

    protected ValueTask SetDataAndCacheAsync(T? data)
    {
        if (Config.CacheData)
            _cacheData = data;

        return LocalStorage.SetItemAsync(Config.Key, data);
    }

    public string KeyName => Config.Key;
    public bool AutoBackup => Config.AutoBackup;

    public void Configure(Action<KeyConfig<T>> configure) => configure(Config);

    public ValueTask<T?> GetAsync() => FetchDataOrCacheAsync();
    public ValueTask<T> GetOrDefaultAsync() => GetOrDefaultAsync(null);
    public async ValueTask<T> GetOrDefaultAsync(Func<T>? defaultConstructor)
    {
        var constructor =
            defaultConstructor ??
            Config?.DefaultConstructor ??
            throw new ArgumentNullException(nameof(defaultConstructor));

        return await GetAsync() ?? constructor();
    }

    public ValueTask SetAsync(T item) => SetDataAndCacheAsync(item);

    public async Task DeleteAsync()
    {
        await LocalStorage.RemoveItemAsync(Config.Key);
        _cacheData = default;
    }

    public void SubscribeToChanges(Func<T, Task> callback)
    {
        LocalStorage.Changed += async (_, args) =>
        {
            if (args.Key == Config.Key)
                await callback((T)args.NewValue);
        };
    }

    public void SubscribeToChangesAsJson(Action<string> callback)
    {
        LocalStorage.Changed += async (_, args) =>
        {
            if (args.Key == Config.Key)
            {
                var json = await DataAsJson();
                if (!string.IsNullOrEmpty(json))
                    callback(json);
            }
        };
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
}
