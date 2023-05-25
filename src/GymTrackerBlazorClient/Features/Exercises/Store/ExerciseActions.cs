using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public record FetchExercisesAction();
public record SetExercisesAction(IEnumerable<Exercise> Exercises);
public record FetchExerciseAction(Guid Id);
public record SetExerciseAction(Exercise Exercise);

public record UpdateExerciseAction(DetailItem Exercise);