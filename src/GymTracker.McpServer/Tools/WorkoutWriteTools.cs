using System.ComponentModel;
using System.Text.Json;
using GymTracker.Domain.Models;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class WorkoutWriteTools
{
    private static readonly JsonSerializerOptions InputJsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions OutputJsonOptions = new() { WriteIndented = true };

    private record WorkoutExerciseInput
    {
        public Guid ExerciseId { get; init; }
        public List<WorkoutSetInput> Sets { get; init; } = [];
    }

    private record WorkoutSetInput
    {
        public string SetType { get; init; } = "Set";
        public bool Completed { get; init; } = false;
        public string? CompletedOn { get; init; }
        public int? Reps { get; init; }
        public decimal? Weight { get; init; }
        public decimal? Time { get; init; }
        public decimal? Distance { get; init; }
    }

    private static (List<WorkoutExercise>? exercises, string? error) ParseWorkoutExercises(
        string json, List<Exercise> allExercises, WorkoutPlan plan)
    {
        List<WorkoutExerciseInput>? inputs;
        try
        {
            inputs = JsonSerializer.Deserialize<List<WorkoutExerciseInput>>(json, InputJsonOptions);
        }
        catch (Exception ex)
        {
            return (null, $"Failed to parse exercisesJson: {ex.Message}");
        }

        if (inputs is null || inputs.Count == 0)
            return ([], null);

        var exercises = new List<WorkoutExercise>();
        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            var exercise = allExercises.FirstOrDefault(e => e.Id == input.ExerciseId);
            if (exercise is null)
                return (null, $"Exercise with id '{input.ExerciseId}' not found.");

            var plannedExercise = plan.PlannedExercises.FirstOrDefault(pe => pe.Exercise.Id == input.ExerciseId);
            if (plannedExercise is null)
                return (null, $"Exercise '{exercise.Name}' is not in workout plan '{plan.Name}'. Every exercise must map to a planned exercise.");

            var workoutExercise = new WorkoutExercise(plannedExercise)
            {
                Order = i + 1
            };

            var sets = new List<WorkoutExerciseSet>();
            var plannedSets = plannedExercise.PlannedSets.OrderBy(s => s.Order).ToList();
            for (var s = 0; s < input.Sets.Count; s++)
            {
                var setInput = input.Sets[s];
                var matchingPlannedSet = s < plannedSets.Count ? plannedSets[s] : null;

                DateTimeOffset? completedOn = null;
                if (setInput.CompletedOn is not null && DateTimeOffset.TryParse(setInput.CompletedOn, out var parsedCompletedOn))
                    completedOn = parsedCompletedOn;

                sets.Add(new WorkoutExerciseSet(matchingPlannedSet)
                {
                    Order = s + 1,
                    SetType = setInput.SetType,
                    Metrics = new ExerciseSetMetrics
                    {
                        Reps = setInput.Reps,
                        Weight = setInput.Weight,
                        Time = setInput.Time,
                        Distance = setInput.Distance
                    },
                    Completed = setInput.Completed,
                    CompletedOn = completedOn
                });
            }

            workoutExercise.Sets = sets;
            exercises.Add(workoutExercise);
        }

        return (exercises, null);
    }

    [McpServerTool(Name = "create_workout")]
    [Description("""
        Record a completed or in-progress workout session. Call get_workout_plans first to get the plan ID and exercise IDs.
        exercisesJson is a JSON array where each element has:
          exerciseId (guid, required — must be an exercise in the specified plan),
          sets: array of { setType ("Warm-up"|"Set"|"Drop-set"), completed (bool), completedOn (ISO 8601, optional), reps (int?), weight (decimal?), time (decimal?), distance (decimal?) }
        workoutStart / workoutEnd: ISO 8601 timestamps e.g. "2026-05-03T10:00:00+01:00". workoutEnd is optional.
        NOTE: Statistics blobs are NOT updated. Workouts will appear in get_workout_history but NOT in get_workout_statistics until the app recalculates them.
        """)]
    public static async Task<string> CreateWorkout(
        GymDataService dataService,
        [Description("GUID of the workout plan used")] string workoutPlanId,
        [Description("When the workout started (ISO 8601)")] string workoutStart,
        [Description("When the workout ended (ISO 8601), optional")] string? workoutEnd = null,
        [Description("Notes about the workout")] string notes = "",
        [Description("JSON array of workout exercises — see tool description for schema")] string exercisesJson = "[]")
    {
        try
        {
            if (!Guid.TryParse(workoutPlanId, out var planId))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutPlanId GUID: '{workoutPlanId}'" });

            if (!DateTimeOffset.TryParse(workoutStart, out var startTime))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutStart: '{workoutStart}'. Use ISO 8601 format." });

            DateTimeOffset? endTime = null;
            if (workoutEnd is not null)
            {
                if (!DateTimeOffset.TryParse(workoutEnd, out var parsedEnd))
                    return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutEnd: '{workoutEnd}'. Use ISO 8601 format." });
                endTime = parsedEnd;
            }

            var plans = await dataService.GetWorkoutPlansAsync();
            var plan = plans.FirstOrDefault(p => p.Id == planId);
            if (plan is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"WorkoutPlan with id '{planId}' not found." });

            var allExercises = await dataService.GetExercisesAsync();
            var (exercises, error) = ParseWorkoutExercises(exercisesJson, allExercises, plan);
            if (error is not null)
                return JsonSerializer.Serialize(new { Success = false, Error = error });

            var workout = new Workout(plan)
            {
                WorkoutStart = startTime,
                WorkoutEnd = endTime,
                Notes = notes,
                Exercises = exercises!
            };

            var workouts = await dataService.GetWorkoutsAsync();
            workouts.Add(workout);
            await dataService.SaveWorkoutsAsync(workouts);

            return JsonSerializer.Serialize(new { Success = true, workout.Id, PlanName = plan.Name }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }

    [McpServerTool(Name = "update_workout")]
    [Description("""
        Full replace of a workout record by ID. All fields are replaced.
        Call get_workout_history to find the workout ID.
        The exercisesJson schema is identical to create_workout.
        WARNING: Statistics blobs are NOT updated. Prefer creating new workouts over editing old ones.
        """)]
    public static async Task<string> UpdateWorkout(
        GymDataService dataService,
        [Description("The GUID of the workout to update")] string workoutId,
        [Description("GUID of the workout plan used")] string workoutPlanId,
        [Description("When the workout started (ISO 8601)")] string workoutStart,
        [Description("When the workout ended (ISO 8601), optional")] string? workoutEnd = null,
        [Description("Notes about the workout")] string notes = "",
        [Description("JSON array of workout exercises — see create_workout for schema")] string exercisesJson = "[]")
    {
        try
        {
            if (!Guid.TryParse(workoutId, out var id))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutId GUID: '{workoutId}'" });

            if (!Guid.TryParse(workoutPlanId, out var planId))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutPlanId GUID: '{workoutPlanId}'" });

            if (!DateTimeOffset.TryParse(workoutStart, out var startTime))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutStart: '{workoutStart}'. Use ISO 8601 format." });

            DateTimeOffset? endTime = null;
            if (workoutEnd is not null)
            {
                if (!DateTimeOffset.TryParse(workoutEnd, out var parsedEnd))
                    return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutEnd: '{workoutEnd}'. Use ISO 8601 format." });
                endTime = parsedEnd;
            }

            var workouts = await dataService.GetWorkoutsAsync();
            var existing = workouts.FirstOrDefault(w => w.Id == id);
            if (existing is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"Workout with id '{id}' not found." });

            var plans = await dataService.GetWorkoutPlansAsync();
            var plan = plans.FirstOrDefault(p => p.Id == planId);
            if (plan is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"WorkoutPlan with id '{planId}' not found." });

            var allExercises = await dataService.GetExercisesAsync();
            var (exercises, error) = ParseWorkoutExercises(exercisesJson, allExercises, plan);
            if (error is not null)
                return JsonSerializer.Serialize(new { Success = false, Error = error });

            var updated = new Workout(plan)
            {
                WorkoutStart = startTime,
                WorkoutEnd = endTime,
                Notes = notes,
                Exercises = exercises!
            } with { Id = existing.Id };

            var index = workouts.IndexOf(existing);
            workouts[index] = updated;
            await dataService.SaveWorkoutsAsync(workouts);

            return JsonSerializer.Serialize(new { Success = true, updated.Id, PlanName = plan.Name }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }

    [McpServerTool(Name = "delete_workout")]
    [Description("""
        Delete a workout record permanently by ID. Does not update statistics blobs.
        Call get_workout_history to find the workout ID.
        NOTE: Statistics blobs will still reference this workout ID — they are not cleaned up automatically.
        """)]
    public static async Task<string> DeleteWorkout(
        GymDataService dataService,
        [Description("The GUID of the workout to delete")] string workoutId)
    {
        try
        {
            if (!Guid.TryParse(workoutId, out var id))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid workoutId GUID: '{workoutId}'" });

            var workouts = await dataService.GetWorkoutsAsync();
            var workout = workouts.FirstOrDefault(w => w.Id == id);
            if (workout is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"Workout with id '{id}' not found." });

            workouts.Remove(workout);
            await dataService.SaveWorkoutsAsync(workouts);

            return JsonSerializer.Serialize(new { Success = true, DeletedId = workout.Id, PlanName = workout.Plan.Name }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }
}
