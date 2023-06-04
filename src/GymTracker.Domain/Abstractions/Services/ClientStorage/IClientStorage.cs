using GymTracker.Domain.Models;
using GymTracker.Domain.Models.ClientStorage;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
public interface IClientStorage : ILocalStorageContext
{
    public IKeyItem<AppSettings> AppSettings { get; init; }
    public IKeyListItem<Exercise> Exercises { get; init; }
}
