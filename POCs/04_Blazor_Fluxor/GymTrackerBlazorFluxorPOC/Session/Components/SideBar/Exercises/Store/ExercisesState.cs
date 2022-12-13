using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Store;
using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Store;

[FeatureState]
public record ExercisesState
{
    public ImmutableList<SessionExercise> Exercises { get; init; } = ImmutableList<SessionExercise>.Empty;
    public Guid? SelectedSessionExerciseId { get; init; } = null;
}