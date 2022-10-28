namespace GymTrackerBlazorFluxorPOC.Session.Main;

public record SessionState
{
    public Guid? SelectedExerciseId { get; init; } = null;
    public DateOnly SessionDate { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string ExerciseName { get; init; } = string.Empty;


}
