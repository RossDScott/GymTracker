namespace GymTracker.BlazorClient.Features.Workout.End.Store;

using Models = GymTracker.Domain.Models;

public record EndWorkoutAction();
public record ConfirmEndWorkoutAction();
public record SetEndWorkoutAction(Models.Workout Workout);