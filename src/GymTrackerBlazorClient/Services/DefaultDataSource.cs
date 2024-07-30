using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;
using System.Net.Http.Json;

namespace GymTracker.BlazorClient.Services;

public class DefaultDataSource : IDefaultDataSource<DefaultData>
{
    private readonly HttpClient _httpClient;

    public DefaultDataSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<DefaultData?> LoadData()
        => _httpClient.GetFromJsonAsync<DefaultData>("defaultdata.json");
}
