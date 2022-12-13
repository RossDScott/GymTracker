using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Services;
using GymTrackerBlazorFluxorPOC.Session.Store;

namespace GymTrackerBlazorFluxorPOC.Session.Components.Main.Components.ExerciseDetail.Store;

public class ExerciseDetailEffects
{
    private readonly SessionService sessionService;
    private readonly IState<SessionState> state;

    public ExerciseDetailEffects(SessionService sessionService, IState<SessionState> state)
    {
        this.sessionService = sessionService;
        this.state = state;
    }

    [EffectMethod]
    public Task LoadExercise(SetSelectedExerciseAction action, IDispatcher dispatcher)
    {
        var session = sessionService.FetchExistingSession(state.Value.Session!.Id);
        var exercise = session.Exercises.Single(x => x.Id == action.SessionExercise.Id);

        dispatcher.Dispatch(new SetExerciseDetailAction(exercise));

        return Task.CompletedTask;
    }
}
