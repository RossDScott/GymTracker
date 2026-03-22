namespace GymTracker.BlazorClient.Features.Workout.End.Store;

using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

public record EndWorkoutAction();
public record ConfirmEndWorkoutAction();
public record CancelEndWorkoutAction();
public record AbandonWorkoutAction();
public record SetEndWorkoutAction(
    Models.Workout Workout,
    Models.Statistics.WorkoutPlanStatistic? PreviousStatistics,
    ImmutableArray<Models.Statistics.ExerciseMilestones> ExerciseMilestones);
public record SetSelectedProgressAction(Guid workoutExerciseId, ProgressType progressType);
public record SetCustomProgressAction(Guid WorkoutExerciseId, Models.ExerciseSetMetrics Metrics);