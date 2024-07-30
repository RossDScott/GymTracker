using Blazored.LocalStorage;

namespace GymTracker.LocalStorage.Core;
public abstract class LocalStorageContext
{
    protected readonly IServiceProvider _serviceProvider = default!;
    protected readonly ILocalStorageService _localStorage = default!;

    public IEnumerable<IKeyItem> Keys { get; internal set; } = Enumerable.Empty<IKeyItem>();

    public IKeyItem<bool> HasInitialisedDefaultData { get; init; } = default!;


    internal virtual void Configure()
    {
        HasInitialisedDefaultData.Configure(settings =>
        {
            settings.AutoBackup = false;
            settings.CacheData = false;
        });
    }

    internal abstract Task InitializeData();

    private readonly ICollection<ITrigger> _triggers = new List<ITrigger>();
    protected void AddTrigger(ITrigger trigger)
    {
        _triggers.Add(trigger);
        trigger.Subscribe();
    }
}