using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;
public abstract class LocalStorageContext : ILocalStorageContext
{
    protected readonly ILocalStorageService _localStorage = default!;

    public IEnumerable<IKeyItem> Keys { get; internal set; } = Enumerable.Empty<IKeyItem>();

    public IKeyItem<bool> HasInitialisedDefaultData { get; init; } = default!;

    internal virtual void Configure()
    {
        HasInitialisedDefaultData.Configure(settings =>
        {
            settings.AutoBackup = false;
        });
    }

    internal abstract Task InitializeData();
}