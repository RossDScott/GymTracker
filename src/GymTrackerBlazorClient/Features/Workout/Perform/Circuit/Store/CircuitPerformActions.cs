namespace GymTracker.BlazorClient.Features.Workout.Perform.Circuit.Store;

public record InitCircuitPerformAction(CircuitPerformState State);
public record CircuitAdvanceAction(int? ActualReps, decimal? ActualWeight, decimal? ActualTime);
public record CircuitSkipRestAction();
public record CircuitSetProgressAction(int CurrentRound, int CurrentExerciseIndex, CircuitPhase Phase);
