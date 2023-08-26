namespace GymTracker.Repository;
public record KeyConfig<T>
{
    public string Key { get; set; } = default!;
    public bool AutoBackup { get; set; } = true;
    public Func<T>? DefaultConstructor { get; set; } = null;
    public bool CacheData { get; set; } = true;
}

public record KeyListItemConfig<T, Item> where T : ICollection<Item>
{
    public Func<Item, Guid> GetId { get; set; } = default!;
}