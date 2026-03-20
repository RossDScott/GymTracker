using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Statistics;
using GymTracker.LocalStorage.Core;
using GymTracker.LocalStorage.Triggers;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.LocalStorage;
public class ClientStorageContext : LocalStorageContext, IClientStorage
{
    public IKeyListItem<Exercise> Exercises { get; init; } = default!;
    public IKeyItem<AppSettings> AppSettings { get; init; } = default!;
    public IKeyListItem<WorkoutPlan> WorkoutPlans { get; init; } = default!;
    public IKeyListItem<Workout> Workouts { get; init; } = default!;
    public IKeyItem<Workout> CurrentWorkout { get; init; } = default!;

    public IKeyListItem<ExerciseStatistic> ExerciseStatistics { get; init; } = default!;
    public IKeyListItem<WorkoutPlanStatistic> WorkoutPlanStatistics { get; init; } = default!;
    public IKeyListItem<WorkoutStatistic> WorkoutStatistics { get; init; } = default!;
    public IKeyItem<NextWorkoutSummary> NextWorkoutSummary { get; init; } = default!;

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

        CurrentWorkout.Configure(settings =>
        {
            settings.CacheData = true;
        });

        Exercises.ConfigureList(settings =>
        {
            settings.GetId = (Exercise x) => x.Id;
        });

        WorkoutPlans.ConfigureList(settings =>
        {
            settings.GetId = (WorkoutPlan x) => x.Id;
        });

        Workouts.ConfigureList(settings =>
        {
            settings.GetId = (Workout x) => x.Id;
        });

        ExerciseStatistics.ConfigureList(settings =>
        {
            settings.GetId = (ExerciseStatistic x) => x.ExerciseId;
        });

        WorkoutPlanStatistics.ConfigureList(settings =>
        {
            settings.GetId = (WorkoutPlanStatistic x) => x.WorkoutPlanId;
        });

        WorkoutStatistics.ConfigureList(settings =>
        {
            settings.GetId = (WorkoutStatistic x) => x.WorkoutId;
        });

        NextWorkoutSummary.Configure(settings =>
        {
            settings.AutoBackup = true;
        });

        AddTrigger(new StatisticsBuilderService(this));
        AddTrigger(new ExerciseUpdateTrigger(this));

        base.Configure();
    }

    internal override async Task InitializeData()
    {
        if (!await HasInitialisedDefaultData.GetAsync())
        {
            var defaultDataBuilder = _serviceProvider.GetRequiredService<DefaultDataBuilderService>();
            await defaultDataBuilder.BuildDefaultData();
        }
    }
}
