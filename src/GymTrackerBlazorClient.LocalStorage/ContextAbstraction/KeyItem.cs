using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;

public class KeyItem<T> : IKeyItem<T>
{
    protected readonly ILocalStorageService _localStorage;
    protected readonly string _key;
    protected Func<T>? _defaultConstructor = null;

    public KeyItem(ILocalStorageService localStorage, IDataBackupService dataBackupService, string key)
    {
        _localStorage = localStorage;
        _key = key;

        if(dataBackupService is not null)
            SubscribeToChanges(data => dataBackupService.BackupAsync(data, _key));
    }

    public IKeyItem<T> ConfigureDefault(Func<T> defaultConstructor)
    {
        _defaultConstructor = defaultConstructor;
        return this;
    }

    public ValueTask<T?> GetAsync() => _localStorage.GetItemAsync<T?>(_key);
    public async ValueTask<T> GetOrDefaultAsync(Func<T> defaultConstructor)
    {
        var val = await _localStorage.GetItemAsync<T?>(_key);
        return val ?? defaultConstructor();
    }

    public async ValueTask<T> GetOrDefaultAsync()
    {
        if(_defaultConstructor is null)
            throw new ArgumentNullException(nameof(_defaultConstructor));

        var data = await GetOrDefaultAsync(_defaultConstructor);
        return data;
    }

    public ValueTask SetAsync(T item) => _localStorage.SetItemAsync(_key, item);

    public void SubscribeToChanges(Action<T> callback)
    {
        _localStorage.Changed += (_, args) =>
        {
            if (args.Key == _key)
                callback((T)args.NewValue);
        };
    }
}
