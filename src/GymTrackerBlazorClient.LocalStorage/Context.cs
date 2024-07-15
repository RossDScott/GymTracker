using GymTracker.BlazorClient.Services;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;

namespace GymTracker.LocalStorage;
public class ClientStorageContext : LocalStorageContext, IClientStorage
{
    public IKeyListItem<Exercise> Exercises { get; init; } = default!;
    public IKeyItem<AppSettings> AppSettings { get; init; } = default!;
    public IKeyListItem<WorkoutPlan> WorkoutPlans { get; init; } = default!;
    public IKeyListItem<Workout> Workouts { get; init; } = default!;
    public IKeyItem<Workout> CurrentWorkout { get; init; } = default!;

    public IKeyListItem<ExerciseStatistic> ExerciseStatistics { get; init; } = default!;

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

        ExerciseStatistics.ConfigureList(settings =>
        {
            settings.GetId = (ExerciseStatistic x) => x.ExerciseId;
        });

        AddTrigger(new StatisticsBuilderService(this));

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
