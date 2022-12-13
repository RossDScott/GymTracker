using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Store;
using GymTrackerBlazorFluxorPOC.Session.Services;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Exercises.Store;

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
        var exercises = currentSession.Exercises!.Select(x => new SessionExercise(x.Id, x.Exercise!.Name)).ToList();
        dispatcher.Dispatch(new SetExercisesAction(exercises));
    }
}
