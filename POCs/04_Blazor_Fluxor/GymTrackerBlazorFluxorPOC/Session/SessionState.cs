namespace GymTrackerBlazorFluxorPOC.Session;

public record SessionState
{
    public Session? Session { get; init; }
    public Exercise? SelectedExercise { get; init; }

    public string Title => $"{Session?.StartedOn.ToString(@"dd/MM/yy")} - {Session?.Name} {(SelectedExercise != null ? " - " + SelectedExercise.Name : "")}";
}

public record Session(Guid Id, string Name, DateTimeOffset StartedOn);
public record Exercise(Guid Id, string Name);