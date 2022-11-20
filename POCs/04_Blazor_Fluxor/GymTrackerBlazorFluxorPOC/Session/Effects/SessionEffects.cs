using Fluxor;
using GymTrackerBlazorFluxorPOC.Session.Actions;
using GymTrackerBlazorFluxorPOC.Session.Services;
using Microsoft.AspNetCore.Components;

namespace GymTrackerBlazorFluxorPOC.Session.Effects;

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
        var workoutPlan = newSession.WorkoutPlan!;

        var session = new Session(newSession.Id, workoutPlan.Name, DateTimeOffset.Now);
        var firstExercise = newSession.Exercises?.FirstOrDefault();

        dispatcher.Dispatch(new SetSessionAction(session));
        if (firstExercise is not null)
            dispatcher.Dispatch(new SetSelectedExerciseAction(new Exercise(firstExercise.Id, firstExercise.Exercise!.Name)));

        navigation.NavigateTo($"Session/{session.Id}");

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task LoadExistingSession(LoadExistingSessionAction action, IDispatcher dispatcher)
    {
        var newSession = sessionService.LoadExistingSession(action.SessionId);
        var workoutPlan = newSession.WorkoutPlan!;

        var session = new Session(newSession.Id, workoutPlan.Name, DateTimeOffset.Now);
        var firstExercise = newSession.Exercises?.FirstOrDefault();

        dispatcher.Dispatch(new SetSessionAction(session));
        if (firstExercise is not null)
            dispatcher.Dispatch(new SetSelectedExerciseAction(new Exercise(firstExercise.Id, firstExercise.Exercise!.Name)));

        navigation.NavigateTo($"Session/{session.Id}");

        return Task.CompletedTask;
    }

    //[EffectMethod]
    //public void SetExercise(SetSelectedExerciseAction action, IDispatcher dispatcher)
    //{
    //    var currentSession = sessionService.CurrentSession!;
    //    var exercise = currentSession.Exercises!.Single(x => x.Id == action.ExerciseId);

    //    dispatcher.Dispatch(new SetExerciseNameAction(exercise.Exercise!.Name));

    //}
}