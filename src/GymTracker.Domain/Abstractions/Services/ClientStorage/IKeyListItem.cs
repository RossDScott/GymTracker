namespace GymTracker.Domain.Abstractions.Services.ClientStorage;

public interface IKeyListItem<T> : IKeyItem<ICollection<T>>
{
    ValueTask<ICollection<T>> GetOrDefaultAsync();
}