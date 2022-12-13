using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session.Store;

[FeatureState]
public record SessionState
{
    public Session? Session { get; init; }
    public SessionExercise? SelectedSessionExercise { get; init; }

    public string Title => $"{Session?.StartedOn.ToString(@"dd/MM/yy")} - {Session?.Name} {(SelectedSessionExercise != null ? " - " + SelectedSessionExercise.Name : "")}";
}

public record Session(Guid Id, string Name, DateTimeOffset StartedOn);
public record SessionExercise(Guid Id, string Name);