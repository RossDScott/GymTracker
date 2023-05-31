using GymTracker.Domain.Models;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
public interface IClientStorage
{
    public IKeyItem<bool> HasInitialisedDefaultData { get; init; }
    public IKeyItem<AppSettings> AppSettings { get; init; }
    public IKeyListItem<Exercise> Exercises { get; init; }
}
