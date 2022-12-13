using GymTrackerBlazorFluxorPOC.Session.Models;
using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.Components.Main.Components.ExerciseDetail.Store;

public record SetExerciseDetailAction(SessionExercise SessionExercise);
public record SetSelectedSetAction(Guid? Id);
public record ToggleSetCompletedAction(Guid Id);
public record SetSetDataAction(EditSet EditSet);