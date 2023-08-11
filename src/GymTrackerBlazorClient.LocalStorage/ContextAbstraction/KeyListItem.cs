using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;

namespace GymTracker.LocalStorage.ContextAbstraction;

public class KeyListItem<T> : KeyItem<ICollection<T>>, IKeyListItem<T> where T : class
{
    protected KeyListItemConfig<ICollection<T>, T> ListConfig = new();

    public KeyListItem(ILocalStorageService localStorage, string key) : base(localStorage, key) 
    {
        ConfigureDefaults();
    }

    public void ConfigureList(Action<KeyListItemConfig<ICollection<T>, T>> configure)
        => configure(ListConfig);

    public async Task<T> FindByIdAsync(Guid id)
        => (await this.GetOrDefaultAsync()).FindById(id, ListConfig);

    public async Task<T?> FindOrDefaultByIdAsync(Guid id)
        => (await this.GetOrDefaultAsync()).FindOrDefaultById(id, ListConfig);

    public async Task<UpsertResponse> UpsertAsync(T item)
    {
        var items = await this.GetOrDefaultAsync();
        var existing = items.FindOrDefaultById(ListConfig.GetId(item), ListConfig);
        if(existing == null)
            items.Add(item);

        await this.SetAsync(items);

        return existing == null ? UpsertResponse.New : UpsertResponse.Existing;
    }
    private void ConfigureDefaults()
    {
        this.Configure(x => x.DefaultConstructor = () => new List<T>());

        var type = typeof(T);
        var idProperty = type.GetProperty("Id", typeof(Guid));
        if (idProperty != null)
        {
            this.ConfigureList(settings =>
            {
                settings.GetId = (item) => (Guid)idProperty.GetValue(item)!;
            });
        }
    }
}

public static class KeyListExtensions
{
    public static T FindById<T>(this ICollection<T> items, Guid id, KeyListItemConfig<ICollection<T>, T> ListConfig)
        => items.Single(x => ListConfig.GetId(x) == id);

    public static T? FindOrDefaultById<T>(this ICollection<T> items, Guid id, KeyListItemConfig<ICollection<T>, T> ListConfig)
        => items.SingleOrDefault(x => ListConfig.GetId(x) == id);

}