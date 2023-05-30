using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;

public class KeyItem<T> : IKeyItem<T>
{
    protected readonly ILocalStorageService _localStorage;
    protected readonly string _key;

    public KeyItem(ILocalStorageService localStorage, IDataBackupService dataBackupService, string key)
    {
        _localStorage = localStorage;
        _key = key;

        if(dataBackupService is not null)
            SubscribeToChanges(data => dataBackupService.BackupAsync(data, _key));
    }

    public ValueTask<T?> GetAsync() => _localStorage.GetItemAsync<T?>(_key);
    public async ValueTask<T> GetOrDefaultAsync(Func<T> defaultConstructor)
    {
        var val = await _localStorage.GetItemAsync<T?>(_key);
        return val ?? defaultConstructor();
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
