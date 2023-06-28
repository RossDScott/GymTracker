using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public record FetchExercisesAction(Guid? SelectedId = null);
public record SetInitialDataAction(
    Guid? SelectedId,
    IEnumerable<string> TargetBodyParts,
    IEnumerable<string> Equipment,
    IEnumerable<Exercise> Exercises);
public record SetExercisesAction(
    IEnumerable<Exercise> Exercises);
public record FetchExerciseAction(Guid Id);
public record SetExerciseAction(Exercise Exercise);

public record AddOrUpdateExerciseAction(DetailItem Exercise);
public record CreateNewExerciseAction();
public record NavigateToNewExerciseAction(Guid Id);

public record UpdateFilterAction(ExercisesFilter Filter);