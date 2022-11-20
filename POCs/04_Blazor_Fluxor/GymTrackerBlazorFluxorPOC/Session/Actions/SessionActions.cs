namespace GymTrackerBlazorFluxorPOC.Session.Actions;

public record CreateNewSessionAction(Guid WorkoutId);
public record LoadExistingSessionAction(Guid SessionId);

public record SetSessionAction(Session Session);

public record SetSelectedExerciseAction(Exercise Exercise);