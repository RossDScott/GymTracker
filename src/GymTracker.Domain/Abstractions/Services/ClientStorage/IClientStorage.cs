using GymTracker.Domain.Models;
using GymTracker.Domain.Models.ClientStorage;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
public interface IClientStorage : ILocalStorageContext
{
    IKeyListItem<string> TargetBodyParts { get; init; }
    IKeyListItem<string> Equipment { get; init; }
    IKeyItem<AppSettings> AppSettings { get; init; }
    IKeyListItem<Exercise> Exercises { get; init; }
}
