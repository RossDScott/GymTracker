using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Actions;
using GymTrackerBlazorFluxorPOC.Session.Services;
using GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Actions;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Effects;

public class ExercisesEffects
{
    private readonly SessionService sessionService;
    private readonly IState<ExercisesState> state;

    public ExercisesEffects(SessionService sessionService, IState<ExercisesState> state)
    {
        this.sessionService = sessionService;
        this.state = state;
    }

    [EffectMethod]
    public async Task LoadExercises(LoadExercisesAction action, IDispatcher dispatcher)
    {
        var currentSession = sessionService.CurrentSession!;
        var exercises = currentSession.Exercises!.Select(x => new Exercise(x.Id, x.Exercise!.Name)).ToList();
        dispatcher.Dispatch(new SetExercisesAction(exercises));
    }
}
