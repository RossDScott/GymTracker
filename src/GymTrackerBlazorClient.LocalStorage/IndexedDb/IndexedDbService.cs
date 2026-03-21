using System.Text.Json;
using Microsoft.JSInterop;

namespace GymTracker.LocalStorage.IndexedDb;

public class IndexedDbService : IIndexedDbService, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    public IndexedDbService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/GymTracker.LocalStorage/indexedDb.js");
        return _module;
    }

    public async ValueTask<T?> GetAsync<T>(string storeName, object key)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<T?>("get", storeName, key);
    }

    public async ValueTask<List<T>> GetAllAsync<T>(string storeName)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<List<T>>("getAll", storeName);
    }

    public async ValueTask PutAsync<T>(string storeName, T value)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("put", storeName, value);
    }

    public async ValueTask PutManyAsync<T>(string storeName, IEnumerable<T> items)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("putMany", storeName, items);
    }

    public async ValueTask DeleteAsync(string storeName, object key)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("del", storeName, key);
    }

    public async ValueTask ClearAsync(string storeName)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("clear", storeName);
    }

    public async ValueTask<int> CountAsync(string storeName)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<int>("count", storeName);
    }

    public async ValueTask<List<T>> GetByIndexAsync<T>(string storeName, string indexName, object key)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<List<T>>("getByIndex", storeName, indexName, key);
    }

    public async ValueTask<JsonElement[]> GetBatchAsync(object[] operations)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<JsonElement[]>("getBatch", new object[] { operations });
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
