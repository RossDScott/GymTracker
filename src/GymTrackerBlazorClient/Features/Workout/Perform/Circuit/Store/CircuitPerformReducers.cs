using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Circuit.Store;

public static class CircuitPerformReducers
{
    [ReducerMethod]
    public static CircuitPerformState OnInit(CircuitPerformState state, InitCircuitPerformAction action)
        => action.State;

    [ReducerMethod]
    public static CircuitPerformState OnSetProgress(CircuitPerformState state, CircuitSetProgressAction action)
        => state with
        {
            CurrentRound = action.CurrentRound,
            CurrentExerciseIndex = action.CurrentExerciseIndex,
            Phase = action.Phase
        };
}
