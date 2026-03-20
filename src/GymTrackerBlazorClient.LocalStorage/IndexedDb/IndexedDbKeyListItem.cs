using GymTracker.LocalStorage.Core;
using System.Text.Json;

namespace GymTracker.LocalStorage.IndexedDb;

public class IndexedDbKeyListItem<T> : IKeyListItem<T> where T : class
{
    private readonly IIndexedDbService _db;
    private KeyConfig<ICollection<T>> Config = new();
    private KeyListItemConfig<ICollection<T>, T> ListConfig = new();

    private ICollection<T>? _cacheData = null;
    private bool _cacheLoaded = false;

    private readonly List<Func<ICollection<T>, Task>> _changeCallbacks = new();
    private readonly List<Func<T, Task>> _itemUpsertCallbacks = new();
    private readonly List<Action<string>> _jsonChangeCallbacks = new();

    public IndexedDbKeyListItem(IIndexedDbService db, string key)
    {
        _db = db;
        Configure(settings => settings.Key = key);
        ConfigureDefaults();
    }

    public string KeyName => Config.Key;
    public bool AutoBackup => Config.AutoBackup;

    public void Configure(Action<KeyConfig<ICollection<T>>> configure) => configure(Config);
    public void ConfigureList(Action<KeyListItemConfig<ICollection<T>, T>> configure) => configure(ListConfig);

    public async ValueTask<ICollection<T>?> GetAsync()
    {
        if (Config.CacheData && _cacheLoaded)
            return _cacheData;

        var items = await _db.GetAllAsync<T>(Config.Key);
        ICollection<T> result = items;

        if (Config.CacheData)
        {
            _cacheData = result;
            _cacheLoaded = true;
        }

        return result;
    }

    public ValueTask<ICollection<T>> GetOrDefaultAsync() => GetOrDefaultAsync(null);

    public async ValueTask<ICollection<T>> GetOrDefaultAsync(Func<ICollection<T>>? defaultConstructor)
    {
        var constructor =
            defaultConstructor ??
            Config?.DefaultConstructor ??
            throw new ArgumentNullException(nameof(defaultConstructor));

        return await GetAsync() ?? constructor();
    }

    public async Task<T> FindByIdAsync(Guid id)
    {
        var item = await _db.GetAsync<T>(Config.Key, id.ToString());
        return item ?? throw new InvalidOperationException($"Item with id {id} not found in {Config.Key}");
    }

    public async Task<T?> FindOrDefaultByIdAsync(Guid id)
    {
        return await _db.GetAsync<T>(Config.Key, id.ToString());
    }

    public async Task UpsertAsync(T item)
    {
        await _db.PutAsync(Config.Key, item);

        InvalidateCache();

        // Fire item-level upsert callbacks (for incremental statistics updates)
        foreach (var callback in _itemUpsertCallbacks)
            await callback(item);

        // Fire JSON change callbacks (for backup orchestrator)
        await NotifyJsonChangeCallbacks();
    }

    public async ValueTask SetAsync(ICollection<T> items)
    {
        await _db.ClearAsync(Config.Key);
        if (items.Count > 0)
            await _db.PutManyAsync(Config.Key, items);

        if (Config.CacheData)
        {
            _cacheData = items;
            _cacheLoaded = true;
        }

        // Fire collection-level change callbacks (for bulk operations like migration/restore)
        foreach (var callback in _changeCallbacks)
            await callback(items);

        await NotifyJsonChangeCallbacks(items);
    }

    public async Task DeleteAsync()
    {
        await _db.ClearAsync(Config.Key);
        InvalidateCache();
    }

    public void SubscribeToChanges(Func<ICollection<T>, Task> callback)
    {
        _changeCallbacks.Add(callback);
    }

    public void SubscribeToItemUpsert(Func<T, Task> callback)
    {
        _itemUpsertCallbacks.Add(callback);
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

        var data = JsonSerializer.Deserialize<ICollection<T>>(jsonString);
        if (data is null)
            return;

        await SetAsync(data);
    }

    private void InvalidateCache()
    {
        _cacheData = null;
        _cacheLoaded = false;
    }

    private async Task NotifyJsonChangeCallbacks(ICollection<T>? items = null)
    {
        if (_jsonChangeCallbacks.Count == 0)
            return;

        items ??= await GetOrDefaultAsync(() => new List<T>());

        var json = JsonSerializer.Serialize(items);
        if (!string.IsNullOrEmpty(json))
        {
            foreach (var callback in _jsonChangeCallbacks)
                callback(json);
        }
    }

    private void ConfigureDefaults()
    {
        Configure(x => x.DefaultConstructor = () => new List<T>());
    }
}
