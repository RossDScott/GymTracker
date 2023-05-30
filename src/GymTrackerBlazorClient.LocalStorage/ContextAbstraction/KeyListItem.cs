using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;

public class KeyListItem<T> : KeyItem<ICollection<T>>, IKeyListItem<T>
{
    public KeyListItem(ILocalStorageService localStorage, IDataBackupService dataBackupService, string key) : base(localStorage, dataBackupService, key) { }

    public async ValueTask<ICollection<T>> GetOrDefaultAsync() => await GetAsync() ?? new List<T>();
}