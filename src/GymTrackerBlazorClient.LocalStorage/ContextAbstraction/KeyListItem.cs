using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;

public class KeyListItem<T> : KeyItem<ICollection<T>>, IKeyListItem<T>
{
    public KeyListItem(ILocalStorageService localStorage, string key) : base(localStorage, key) 
    {
        this.Configure(x => x.DefaultConstructor = () => new List<T>());
    }
}