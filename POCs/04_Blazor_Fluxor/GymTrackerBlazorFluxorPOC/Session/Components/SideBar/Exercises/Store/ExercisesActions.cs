using GymTrackerBlazorFluxorPOC.Session.Store;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Store;

public record LoadExercisesAction();
public record SetExercisesAction(List<SessionExercise> Exercises);