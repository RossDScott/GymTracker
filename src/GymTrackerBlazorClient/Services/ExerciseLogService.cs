//using GymTracker.Domain.Models;
//using GymTracker.Repository;

//namespace GymTracker.BlazorClient.Services;

//public class ExerciseLogService
//{
//    private readonly IClientStorage _clientStorage;

//    public ExerciseLogService(IClientStorage clientStorage)
//    {
//        _clientStorage = clientStorage;

//        clientStorage.Workouts.SubscribeToChanges(WorkoutsChanged);
//    }

//    public void WorkoutsChanged(ICollection<Workout> workouts)
//    {
//        var completedWorkouts = workouts.Where(x => x.WorkoutEnd != null).ToList();


//    }
//}
