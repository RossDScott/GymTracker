using Blazored.LocalStorage;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;

namespace GymTracker.LocalStorage.IndexedDb;

public class DataMigrationService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IIndexedDbService _indexedDb;

    public DataMigrationService(ILocalStorageService localStorage, IIndexedDbService indexedDb)
    {
        _localStorage = localStorage;
        _indexedDb = indexedDb;
    }

    public async Task MigrateIfNeededAsync(ClientStorageContext context)
    {
        // Check if already migrated
        var meta = await _indexedDb.GetAsync<MetaRecord>("Meta", "migrated_from_localstorage");
        if (meta is not null)
            return;

        // Check if there's any data in localStorage to migrate
        var hasData = await _localStorage.GetItemAsync<bool?>("HasInitialisedDefaultData");
        if (hasData is not true)
            return; // No localStorage data to migrate

        Console.WriteLine("Migrating data from localStorage to IndexedDB...");

        await MigrateListAsync<Exercise>("Exercises");
        await MigrateListAsync<WorkoutPlan>("WorkoutPlans");
        await MigrateListAsync<Workout>("Workouts");
        await MigrateListAsync<ExerciseStatistic>("ExerciseStatistics");
        await MigrateListAsync<WorkoutPlanStatistic>("WorkoutPlanStatistics");
        await MigrateListAsync<WorkoutStatistic>("WorkoutStatistics");

        await MigrateSingletonAsync<AppSettings>("AppSettings");
        await MigrateSingletonAsync<Workout>("CurrentWorkout");
        await MigrateSingletonAsync<bool>("HasInitialisedDefaultData");

        // Mark migration as complete
        await _indexedDb.PutAsync("Meta", new MetaRecord { Key = "migrated_from_localstorage", Value = DateTimeOffset.Now.ToString("O") });

        Console.WriteLine("Migration from localStorage to IndexedDB complete.");
    }

    private async Task MigrateListAsync<T>(string key) where T : class
    {
        try
        {
            var items = await _localStorage.GetItemAsync<List<T>>(key);
            if (items is not null && items.Count > 0)
            {
                await _indexedDb.PutManyAsync(key, items);
                Console.WriteLine($"  Migrated {items.Count} items from '{key}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Warning: Failed to migrate '{key}': {ex.Message}");
        }
    }

    private async Task MigrateSingletonAsync<T>(string key)
    {
        try
        {
            var item = await _localStorage.GetItemAsync<T>(key);
            if (item is not null)
            {
                var wrapper = new SingletonWrapper<T> { Value = item };
                await _indexedDb.PutAsync(key, wrapper);
                Console.WriteLine($"  Migrated singleton '{key}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Warning: Failed to migrate singleton '{key}': {ex.Message}");
        }
    }

    internal record MetaRecord
    {
        public string Key { get; init; } = default!;
        public string Value { get; init; } = default!;
    }
}
