using Blazored.LocalStorage;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Statistics;

namespace GymTracker.LocalStorage.IndexedDb;

public record StorageDiagnostics(
    bool MigrationComplete,
    IReadOnlyList<StoreCount> Counts
);

public record StoreCount(string Name, int LocalStorage, int IndexedDb)
{
    public bool Matches => LocalStorage == IndexedDb;
}

public class StorageDiagnosticsService
{
    private readonly IIndexedDbService _indexedDb;
    private readonly ILocalStorageService _localStorage;

    public StorageDiagnosticsService(IIndexedDbService indexedDb, ILocalStorageService localStorage)
    {
        _indexedDb = indexedDb;
        _localStorage = localStorage;
    }

    public async Task<StorageDiagnostics> GetDiagnosticsAsync()
    {
        var migrationMeta = await _indexedDb.GetAsync<DataMigrationService.MetaRecord>("Meta", "migrated_from_localstorage");
        var migrationComplete = migrationMeta is not null;

        var counts = new List<StoreCount>
        {
            await GetCountAsync<Exercise>("Exercises"),
            await GetCountAsync<WorkoutPlan>("WorkoutPlans"),
            await GetCountAsync<Workout>("Workouts"),
            await GetCountAsync<ExerciseStatistic>("ExerciseStatistics"),
            await GetCountAsync<WorkoutPlanStatistic>("WorkoutPlanStatistics"),
            await GetCountAsync<WorkoutStatistic>("WorkoutStatistics"),
        };

        return new StorageDiagnostics(migrationComplete, counts);
    }

    private async Task<StoreCount> GetCountAsync<T>(string storeName) where T : class
    {
        var localStorageCount = 0;
        try
        {
            var items = await _localStorage.GetItemAsync<List<T>>(storeName);
            localStorageCount = items?.Count ?? 0;
        }
        catch { }

        var indexedDbCount = await _indexedDb.CountAsync(storeName);

        return new StoreCount(storeName, localStorageCount, indexedDbCount);
    }
}
