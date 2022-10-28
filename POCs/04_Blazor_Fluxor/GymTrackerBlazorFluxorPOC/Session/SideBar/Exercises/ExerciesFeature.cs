using Fluxor;
using System.Collections.Immutable;

namespace GymTrackerBlazorFluxorPOC.Session.SideBar.Exercises;

public class ExerciesFeature : Feature<ExercisesState>
{
    public override string GetName() => "Exercises";

    protected override ExercisesState GetInitialState()
    {
        return new ExercisesState();
    }
}
