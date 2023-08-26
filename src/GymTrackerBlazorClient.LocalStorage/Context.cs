using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;
using GymTracker.Repository;

namespace GymTracker.LocalStorage;
public class ClientStorageContext : LocalStorageContext, IClientStorage
{
    public IKeyListItem<Exercise> Exercises { get; init; } = default!;
    public IKeyItem<AppSettings> AppSettings { get; init; } = default!;
    public IKeyListItem<WorkoutPlan> WorkoutPlans { get; init; } = default!;

    public ClientStorageContext()
    {

    }

    internal override void Configure()
    {
        AppSettings.Configure(settings =>
        {
            settings.DefaultConstructor = () => new AppSettings();
            settings.AutoBackup = false;
        });

        base.Configure();
    }

    internal override async Task InitializeData()
    {
        if (!await HasInitialisedDefaultData.GetAsync())
        {
            var defaultData = new DefaultDataBuilderService(this);
            await defaultData.BuildDefaultData();
        }
    }
}
