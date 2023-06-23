using GymTracker.Domain.Models;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models.ClientStorage;
using System.Collections.Immutable;

namespace GymTracker.Domain.Services;
public class DefaultDataBuilderService
{
    private readonly IClientStorage _clientStorage;

    public DefaultDataBuilderService(IClientStorage localStorageContext)
    {
        _clientStorage = localStorageContext;
    }

    public async Task BuildDefaultData()
    {
        await BuildAppSettings();
        await BuildExercises();

        await _clientStorage.HasInitialisedDefaultData.SetAsync(true);
    }

    private async Task BuildAppSettings()
    {
        var settings = new AppSettings
        {
            TargetBodyParts = BuildBodyTargets().ToImmutableArray(),
            Equipment = BuildEquipment().ToImmutableArray(),
            SetType = BuildSetTypes().ToImmutableArray()
        };
        await _clientStorage.AppSettings.SetAsync(settings);
    }

    private string[] BuildBodyTargets() => new string[]
    {
        "Core", "Arms", "Back", "Chest", "Legs", "Shoulders"
    };

    private string[] BuildEquipment() => new string[]
    {
        "Barbell", "Dumbbel", "Pulley", "Spin Bike", "Treadmill", "Floor"
    };

    private string[] BuildSetTypes() => new string[]
    {
        "Warm-up", "Set", "Drop-set"
    };

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
