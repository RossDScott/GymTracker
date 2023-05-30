using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.ContextAbstraction;

namespace GymTracker.LocalStorage;
public class ClientStorageContext : LocalStorageContext, IClientStorage
{
    public IKeyItem<bool> HasInitialisedDefaultData { get; init; } = default!;
    public IKeyListItem<Exercise> Exercises { get; init; } = default!;
    public IKeyItem<string> AzureBlobBackupContainerSASURI { get; init; } = default!;
}
