namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Actions;

public record LoadExercisesAction();
public record SetExercisesAction(List<Exercise> Exercises);