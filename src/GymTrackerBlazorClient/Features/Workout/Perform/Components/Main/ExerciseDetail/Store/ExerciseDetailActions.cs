using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Components.Main.ExerciseDetail.Store;

public record SetExerciseDetailAction(WorkoutExercise WorkoutExercise);
public record SetSetTypesAction(ImmutableArray<string> SetTypes);
public record SetSelectedSetAction(Guid? Id);
public record ToggleSetCompletedAction(Guid WorkoutExerciseId, Guid SetId, bool AutoUnselect);
public record SetSetDataAction(Guid WorkoutExerciseId, EditSet EditSet);
public record SetWeightIncrementAction(Guid WorkoutExerciseId, decimal WeightIncrement);
public record DeleteSetAction(Guid WorkoutExerciseId, Guid SetId);
public record AddSetAction(Guid WorkoutExerciseId, string SetType);