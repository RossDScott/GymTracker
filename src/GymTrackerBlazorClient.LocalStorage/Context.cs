using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.ContextAbstraction;

namespace GymTracker.LocalStorage;
public class ClientStorageContext : LocalStorageContext, IClientStorage
{
    public IKeyItem<bool> HasInitialisedDefaultData { get; init; } = default!;
    public IKeyListItem<Exercise> Exercises { get; init; } = default!;
    public IKeyItem<AppSettings> AppSettings { get; init; } = default!;

    internal override void Configure()
    {
        AppSettings.ConfigureDefault(() => new AppSettings());
    }
}
