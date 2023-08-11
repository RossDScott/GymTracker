using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
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