using GymTracker.Domain.LocalStorage;
using GymTracker.Domain.Models;
using GymTracker.BlazorClient.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Services;
public class DefaultDataBuilderService
{
    private readonly GymTrackerLocalStorageContext _localStorageContext;

    public DefaultDataBuilderService(GymTrackerLocalStorageContext localStorageContext)
    {
        _localStorageContext = localStorageContext;
    }
    public async Task CheckAndBuildDefaultDataIfRequired()
    {
        if (!await _localStorageContext.HasInitialisedDefaultData.GetAsync())
            await BuildDefaultData();
    }

    public async Task BuildDefaultData()
    {
        await BuildExercises();

        await _localStorageContext.HasInitialisedDefaultData.SetAsync(true);
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

        await _localStorageContext.Exercises.SetAsync(exercises);
    }
}
