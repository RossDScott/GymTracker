using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Services;
using Microsoft.AspNetCore.Components;

namespace GymTrackerBlazorFluxorPOC.Session.Store;

public class SessionEffects
{
    private readonly SessionService sessionService;
    private readonly IState<SessionState> state;
    private readonly NavigationManager navigation;

    public SessionEffects(SessionService sessionService, IState<SessionState> state, NavigationManager navigation)
    {
        this.sessionService = sessionService;
        this.state = state;
        this.navigation = navigation;
    }

    [EffectMethod]
    public Task CreateNewSession(CreateNewSessionAction action, IDispatcher dispatcher)
    {
        var newSession = sessionService.CreateNewSession(action.WorkoutId);
        dispatcher.Dispatch(new LoadExistingSessionAction(newSession.Id));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task LoadExistingSession(LoadExistingSessionAction action, IDispatcher dispatcher)
    {
        var newSession = sessionService.FetchExistingSession(action.SessionId);
        var workoutPlan = newSession.WorkoutPlan!;

        var session = new Session(newSession.Id, workoutPlan.Name, DateTimeOffset.Now);

        dispatcher.Dispatch(new SetSessionAction(session));
 
        navigation.NavigateTo($"Session/{session.Id}");

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task SetSelectedExerciseForNewSession(SetSessionAction action, IDispatcher dispatcher)
    {
        var session = sessionService.FetchExistingSession(action.Session.Id);
        var firstExercise = session.Exercises?.FirstOrDefault();

        if (firstExercise is not null)
            dispatcher.Dispatch(new SetSelectedExerciseAction(new SessionExercise(firstExercise.Id, firstExercise.Exercise!.Name)));

        return Task.CompletedTask;
    }
}