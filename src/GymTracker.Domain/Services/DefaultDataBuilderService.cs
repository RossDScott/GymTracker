﻿using GymTracker.Domain.Models;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models.ClientStorage;

namespace GymTracker.Domain.Services;
public class DefaultDataBuilderService
{
    private readonly IClientStorage _clientStorage;

    public DefaultDataBuilderService(IClientStorage localStorageContext)
    {
        _clientStorage = localStorageContext;
    }
    public async Task CheckAndBuildDefaultDataIfRequired()
    {
        if (!await _clientStorage.HasInitialisedDefaultData.GetAsync())
            await BuildDefaultData();
    }

    public async Task BuildDefaultData()
    {
        await BuildExercises();

        var settings = new AppSettings();
        await _clientStorage.AppSettings.SetAsync(settings);

        await _clientStorage.HasInitialisedDefaultData.SetAsync(true);
    }

    private async Task BuildExercises()
    {
        var exercises = new List<Exercise>
        {
            new Exercise
            {
                Name = "Barbell Deadlift",
                MetricType = MetricType.Weight,
                BodyTarget = new [] {"Back", "Glutes", "Hamstrings", "Core"}
            },
            new Exercise
            {
                Name = "Barbell Bench Press",
                MetricType = MetricType.Weight,
                BodyTarget = new [] {"Chest", "Arms"}
            },
            new Exercise
            {
                Name = "Plank",
                MetricType = MetricType.Time,
                BodyTarget = new [] {"Core"}
            }
        };

        exercises = exercises.OrderBy(x => x.Name).ToList();

        await _clientStorage.Exercises.SetAsync(exercises);
    }
}
