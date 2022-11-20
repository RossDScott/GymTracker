using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises;

public record ExercisesState
{
    public ImmutableList<Exercise> Exercises { get; init; } = ImmutableList<Exercise>.Empty;
    public Guid? SelectedExerciseId { get; init; } = null;
}