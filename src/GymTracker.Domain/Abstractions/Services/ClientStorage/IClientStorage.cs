using GymTracker.Domain.Models;
using GymTracker.Domain.Models.ClientStorage;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
public interface IClientStorage : ILocalStorageContext
{
    IKeyItem<AppSettings> AppSettings { get; init; }
    IKeyListItem<Exercise> Exercises { get; init; }
}
