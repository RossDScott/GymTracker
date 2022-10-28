using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Actions;
using GymTrackerBlazorFluxorPOC.Session.Main;

namespace GymTrackerBlazorFluxorPOC.Session.Main.Title.Effects;

public class SessionEffects
{
    private readonly SessionService sessionService;
    private readonly IState<SessionState> state;

    public SessionEffects(SessionService sessionService, IState<SessionState> state)
    {
        this.sessionService = sessionService;
        this.state = state;
    }

    [EffectMethod]
    public async Task SetExercise(SetSelectedExerciseAction action, IDispatcher dispatcher)
    {
        var currentSession = sessionService.CurrentSession!;
        var exercises = currentSession.Exercises!.Single(x => x.Id == action.exerciseId);

    }
}