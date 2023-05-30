namespace GymTracker.Domain.Abstractions.Services.ClientStorage;

public interface IKeyItem<T>
{
    ValueTask<T?> GetAsync();
    ValueTask<T> GetOrDefaultAsync(Func<T> defaultConstructor);
    ValueTask SetAsync(T item);
    void SubscribeToChanges(Action<T> callback);
}