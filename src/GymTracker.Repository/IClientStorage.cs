using GymTracker.Domain.Models;

namespace GymTracker.Repository;
public interface IClientStorage
{
    IKeyItem<AppSettings> AppSettings { get; init; }
    IKeyListItem<Exercise> Exercises { get; init; }
    IKeyListItem<WorkoutPlan> WorkoutPlans { get; init; }
}
