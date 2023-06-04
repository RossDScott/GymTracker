using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using System.Text.Json;

namespace GymTracker.LocalStorage.ContextAbstraction;

public class KeyItem<T> : IKeyItem<T>
{
    protected readonly ILocalStorageService LocalStorage;
    protected readonly string Key;
    protected KeyConfig<T> Config = new();

    public KeyItem(ILocalStorageService localStorage, string key)
    {
        LocalStorage = localStorage;
        Key = key;
    }

    public string KeyName => Key;
    public bool AutoBackup => Config.AutoBackup;

    public void Configure(Action<KeyConfig<T>> configure) => configure(Config);
    
    public ValueTask<T?> GetAsync() => LocalStorage.GetItemAsync<T?>(Key);
    public async ValueTask<T> GetOrDefaultAsync()
    {
        var data = await GetOrDefaultAsync(null);
        return data;
    }
    public async ValueTask<T> GetOrDefaultAsync(Func<T>? defaultConstructor)
    {
        var constructor = 
            defaultConstructor ?? 
            Config?.DefaultConstructor ?? 
            throw new ArgumentNullException(nameof(defaultConstructor));

        var val = await LocalStorage.GetItemAsync<T?>(Key);
        return val ?? constructor();
    }

    public ValueTask SetAsync(T item) => LocalStorage.SetItemAsync(Key, item);

    public void SubscribeToChanges(Action<T> callback)
    {
        LocalStorage.Changed += (_, args) =>
        {
            if (args.Key == Key)
                callback((T)args.NewValue);
        };
    }

    public void SubscribeToChangesAsJson(Action<string> callback)
    {
        LocalStorage.Changed += async (_, args) =>
        {
            if (args.Key == Key)
            {
                var json = await DataAsJson();
                if(!string.IsNullOrEmpty(json))
                    callback(json);
            }
        };
    }

    public async ValueTask<string?> DataAsJson()
    {
        var data = await GetAsync();
        return data is null ? null : JsonSerializer.Serialize<T>(data);
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
