using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.ClientStorage;
using GymTracker.Domain.Services;
using GymTracker.LocalStorage.ContextAbstraction;

namespace GymTracker.LocalStorage;
public class ClientStorageContext : LocalStorageContext, IClientStorage
{
    public IKeyListItem<string> TargetBodyParts { get; init; } = default!;
    public IKeyListItem<string> Equipment { get; init; } = default!;

    public IKeyListItem<Exercise> Exercises { get; init; } = default!;
    public IKeyItem<AppSettings> AppSettings { get; init; } = default!;

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
