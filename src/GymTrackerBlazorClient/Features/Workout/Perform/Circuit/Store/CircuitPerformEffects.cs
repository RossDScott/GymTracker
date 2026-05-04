using Fluxor;
using GymTracker.BlazorClient.Extensions;
using GymTracker.BlazorClient.Features.Workout.End.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Components.SideBar.Timers.CountdownTimer.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Components.SideBar.Timers.Stopwatch.Store;
using GymTracker.BlazorClient.Features.Workout.Perform.Store;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Workout.Perform.Circuit.Store;

public class CircuitPerformEffects
{
    private readonly IClientStorage _clientStorage;
    private readonly IState<CircuitPerformState> _circuitState;

    public CircuitPerformEffects(
        IClientStorage clientStorage,
        IState<CircuitPerformState> circuitState)
    {
        _clientStorage = clientStorage;
        _circuitState = circuitState;
    }

    [EffectMethod]
    public Task OnSetWorkout(SetWorkoutAction action, IDispatcher dispatcher)
    {
        if (action.workout.Plan.WorkoutType != WorkoutType.Circuit) return Task.CompletedTask;

        var workout = action.workout;
        var config = workout.Plan.CircuitConfig!;

        var exercises = workout.Exercises
            .OrderBy(e => e.Order)
            .Select(e =>
            {
                var firstSet = e.Sets.OrderBy(s => s.Order).FirstOrDefault();
                return new CircuitExerciseItem
                {
                    WorkoutExerciseId = e.Id,
                    ExerciseName = e.Exercise.Name,
                    MetricType = e.Exercise.MetricType,
                    TargetReps = firstSet?.PlannedExerciseSet?.TargetMetrics.Reps,
                    TargetWeight = firstSet?.PlannedExerciseSet?.TargetMetrics.Weight,
                    TargetTime = firstSet?.PlannedExerciseSet?.TargetMetrics.Time
                };
            })
            .ToImmutableArray();

        var completedPerExercise = workout.Exercises
            .OrderBy(e => e.Order)
            .Select(e => e.Sets.Count(s => s.Completed))
            .ToList();

        int currentRound;
        int currentExerciseIndex;

        var minCompleted = completedPerExercise.Count > 0 ? completedPerExercise.Min() : 0;
        var allEqual = completedPerExercise.All(c => c == minCompleted);

        if (allEqual && minCompleted >= config.Rounds)
        {
            currentRound = config.Rounds;
            currentExerciseIndex = exercises.Length > 0 ? exercises.Length - 1 : 0;
        }
        else if (allEqual)
        {
            currentRound = minCompleted + 1;
            currentExerciseIndex = 0;
        }
        else
        {
            currentRound = minCompleted + 1;
            currentExerciseIndex = completedPerExercise.IndexOf(minCompleted);
        }

        var circuitState = new CircuitPerformState
        {
            WorkoutId = workout.Id,
            PlanName = workout.Plan.Name,
            TotalRounds = config.Rounds,
            RestBetweenRounds = config.RestBetweenRounds,
            CurrentRound = currentRound,
            CurrentExerciseIndex = currentExerciseIndex,
            Phase = CircuitPhase.Exercising,
            Exercises = exercises
        };

        dispatcher.Dispatch(new InitCircuitPerformAction(circuitState));
        dispatcher.Dispatch(new StopwatchStartAction());

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task OnCircuitAdvance(CircuitAdvanceAction action, IDispatcher dispatcher)
    {
        var state = _circuitState.Value;
        var workout = await _clientStorage.CurrentWorkout.GetAsync();
        ArgumentNullException.ThrowIfNull(workout);

        var currentExercise = workout.Exercises
            .OrderBy(e => e.Order)
            .ElementAt(state.CurrentExerciseIndex);

        var currentSet = currentExercise.Sets
            .OrderBy(s => s.Order)
            .ElementAt(state.CurrentRound - 1);

        currentSet.Completed = true;
        currentSet.CompletedOn = DateTimeOffset.Now;
        if (action.ActualReps.HasValue)
            currentSet.Metrics.Reps = action.ActualReps;
        if (action.ActualWeight.HasValue)
            currentSet.Metrics.Weight = action.ActualWeight;
        if (action.ActualTime.HasValue)
            currentSet.Metrics.Time = action.ActualTime;

        await _clientStorage.CurrentWorkout.SetAsync(workout);

        dispatcher.Dispatch(new CountdownTimerResetAction());

        if (state.CurrentExerciseIndex < state.Exercises.Length - 1)
        {
            dispatcher.Dispatch(new CircuitSetProgressAction(
                state.CurrentRound, state.CurrentExerciseIndex + 1, CircuitPhase.Exercising));
        }
        else if (state.CurrentRound < state.TotalRounds)
        {
            dispatcher.Dispatch(new CircuitSetProgressAction(
                state.CurrentRound, 0, CircuitPhase.Resting));
            dispatcher.DispatchWithDelay(
                new CountdownTimerStartWithDurationAction(state.RestBetweenRounds), 500);
        }
        else
        {
            dispatcher.Dispatch(new EndWorkoutAction());
        }
    }

    [EffectMethod]
    public Task OnCircuitSkipRest(CircuitSkipRestAction action, IDispatcher dispatcher)
    {
        var state = _circuitState.Value;
        dispatcher.Dispatch(new CountdownTimerResetAction());
        dispatcher.Dispatch(new CircuitSetProgressAction(
            state.CurrentRound + 1, 0, CircuitPhase.Exercising));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task OnCountdownTimesUp(CountdownTimerTimesUpAction action, IDispatcher dispatcher)
    {
        var state = _circuitState.Value;
        if (!state.IsInitialized || state.Phase != CircuitPhase.Resting)
            return Task.CompletedTask;

        dispatcher.DispatchWithDelay(new CircuitSetProgressAction(
            state.CurrentRound + 1, 0, CircuitPhase.Exercising), 500);
        return Task.CompletedTask;
    }
}
