using GymTracker.BlazorClient.Features.Exercises.Store;
using GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public record FetchWorkoutPlansAction(Guid? SelectedId = null);
public record SetInitialDataAction(
    Guid? SelectedPlanId,
    Guid? SelectedTargetExerciseId,
    IEnumerable<WorkoutPlan> WorkoutPlans,
    IEnumerable<Exercise> Exercises);
public record AddOrUpdateWorkoutPlanAction(DetailItem Exercise);
public record CreateNewWorkoutPlanAction();
public record NavigateToNewWorkoutPlanAction(Guid Id);