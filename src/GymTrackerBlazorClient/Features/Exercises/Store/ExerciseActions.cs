using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public record FetchExercisesAction();
public record SetInitialDataAction(
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

public record FilterBySearchTermAction(string SearchTerm);
public record ToggleFilterByBodyTargetAction(CheckItem BodyTargetItem);
public record FilterByIsActiveAction(ActiveFilterOption ActiveOption);