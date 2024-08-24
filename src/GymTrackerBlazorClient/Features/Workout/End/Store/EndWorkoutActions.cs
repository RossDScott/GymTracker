namespace GymTracker.BlazorClient.Features.Workout.End.Store;

using Models = GymTracker.Domain.Models;

public record EndWorkoutAction();
public record ConfirmEndWorkoutAction();
public record CancelEndWorkoutAction();
public record AbandonWorkoutAction();
public record SetEndWorkoutAction(Models.Workout Workout, Models.Statistics.WorkoutPlanStatistic? PreviousStatistics);
public record SetSelectedProgressAction(Guid workoutExerciseId, ProgressType progressType);