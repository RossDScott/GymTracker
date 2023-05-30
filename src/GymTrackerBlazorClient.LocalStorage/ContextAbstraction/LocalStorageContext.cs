using Blazored.LocalStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;
public abstract class LocalStorageContext
{
    protected readonly ILocalStorageService _localStorage = default!;
}