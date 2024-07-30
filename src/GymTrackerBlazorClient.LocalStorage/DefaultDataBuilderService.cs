using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;
using System.Collections.Immutable;

namespace GymTracker.LocalStorage;
public class DefaultDataBuilderService
{
    private readonly ClientStorageContext _clientStorage;
    private readonly IDefaultDataSource<DefaultData> _defaultDataSource;

    public DefaultDataBuilderService(
        ClientStorageContext localStorageContext,
        IDefaultDataSource<DefaultData> defaultDataSource)
    {
        _clientStorage = localStorageContext;
        _defaultDataSource = defaultDataSource;
    }

    public async Task BuildDefaultData()
    {
        var defaultData = await _defaultDataSource.LoadData();

        ArgumentNullException.ThrowIfNull(defaultData);

        await BuildAppSettings(defaultData);
        await BuildExercises(defaultData);

        await _clientStorage.HasInitialisedDefaultData.SetAsync(true);
    }

    private async Task BuildAppSettings(DefaultData defaultData)
    {
        var settings = new AppSettings
        {
            TargetBodyParts = defaultData.TargetBodyParts.ToImmutableArray(),
            Equipment = defaultData.Equipment.ToImmutableArray(),
            SetType = defaultData.SetTypes.ToImmutableArray()
        };
        await _clientStorage.AppSettings.SetAsync(settings);
    }

    private async Task BuildExercises(DefaultData defaultData)
    {
        var exercises = defaultData.Exercises.OrderBy(x => x.Name).ToList();
        await _clientStorage.Exercises.SetAsync(exercises);
    }
}
