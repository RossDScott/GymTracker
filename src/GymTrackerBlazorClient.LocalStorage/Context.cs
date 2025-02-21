﻿using GymTracker.Domain.Models;
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

        WorkoutPlanStatistics.ConfigureList(settings =>
        {
            settings.GetId = (WorkoutPlanStatistic x) => x.WorkoutPlanId;
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
