using Fluxor;

namespace GymTracker.BlazorClient.Features.Workout.Store;

public static class WorkoutReducers
{
    [ReducerMethod]
    public static WorkoutState OnFetchWorkoutPlans(WorkoutState state, FetchWorkoutPlansAction action) =>
        state with { WorkoutPlanOrder = action.Order };

    [ReducerMethod]
    public static WorkoutState OnSetWorkoutPlans(WorkoutState state, SetWorkoutPlansAction action) =>
        state with { WorkoutPlans = action.WorkoutPlans };

    [ReducerMethod]
    public static WorkoutState OnFetchWorkoutPlansAction(WorkoutState state, FetchWorkoutPlansAction action)
        => state with { WorkoutPlanOrder = action.Order };
}
