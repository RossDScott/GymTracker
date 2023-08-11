using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.ExercisePicker.Store;

public record FetchExercisesAction();
public record SetInitialDataAction(
    IEnumerable<string> TargetBodyParts,
    IEnumerable<string> Equipment,
    IEnumerable<Exercise> Exercises);
public record SetExercisesAction(
    IEnumerable<Exercise> Exercises);

public record UpdateFilterAction(ExercisesFilter Filter);