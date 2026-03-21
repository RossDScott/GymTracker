namespace GymTracker.LocalStorage.IndexedDb;

public interface IIndexedDbService
{
    ValueTask<T?> GetAsync<T>(string storeName, object key);
    ValueTask<List<T>> GetAllAsync<T>(string storeName);
    ValueTask PutAsync<T>(string storeName, T value);
    ValueTask PutManyAsync<T>(string storeName, IEnumerable<T> items);
    ValueTask DeleteAsync(string storeName, object key);
    ValueTask ClearAsync(string storeName);
    ValueTask<int> CountAsync(string storeName);
    ValueTask<List<T>> GetByIndexAsync<T>(string storeName, string indexName, object key);
    ValueTask<System.Text.Json.JsonElement[]> GetBatchAsync(object[] operations);
}
