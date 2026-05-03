using System.ComponentModel;
using System.Text.Json;
using GymTracker.Domain.Models;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class WorkoutPlanWriteTools
{
    private static readonly JsonSerializerOptions InputJsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions OutputJsonOptions = new() { WriteIndented = true };

    private record PlannedExerciseInput
    {
        public Guid ExerciseId { get; init; }
        public int RestIntervalSeconds { get; init; } = 120;
        public bool AutoTriggerRestTimer { get; init; } = true;
        public int TargetRepsLower { get; init; } = 8;
        public int TargetRepsUpper { get; init; } = 12;
        public decimal TargetWeightIncrement { get; init; } = 2.5m;
        public List<PlannedSetInput> Sets { get; init; } = [];
    }

    private record PlannedSetInput
    {
        public string SetType { get; init; } = "Set";
        public int? Reps { get; init; }
        public decimal? Weight { get; init; }
        public decimal? Time { get; init; }
        public decimal? Distance { get; init; }
    }

    private static (List<PlannedExercise>? exercises, string? error) ParsePlannedExercises(
        string json, List<Exercise> allExercises)
    {
        List<PlannedExerciseInput>? inputs;
        try
        {
            inputs = JsonSerializer.Deserialize<List<PlannedExerciseInput>>(json, InputJsonOptions);
        }
        catch (Exception ex)
        {
            return (null, $"Failed to parse plannedExercisesJson: {ex.Message}");
        }

        if (inputs is null || inputs.Count == 0)
            return ([], null);

        var exercises = new List<PlannedExercise>();
        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            var exercise = allExercises.FirstOrDefault(e => e.Id == input.ExerciseId);
            if (exercise is null)
                return (null, $"Exercise with id '{input.ExerciseId}' not found.");

            var setTypeCounters = new Dictionary<string, int>();
            var sets = new List<PlannedExerciseSet>();
            for (var s = 0; s < input.Sets.Count; s++)
            {
                var setInput = input.Sets[s];
                setTypeCounters.TryGetValue(setInput.SetType, out var typeCount);
                setTypeCounters[setInput.SetType] = typeCount + 1;

                sets.Add(new PlannedExerciseSet
                {
                    Order = s + 1,
                    SetType = setInput.SetType,
                    OrderForSetType = typeCount + 1,
                    TargetMetrics = new ExerciseSetMetrics
                    {
                        Reps = setInput.Reps,
                        Weight = setInput.Weight,
                        Time = setInput.Time,
                        Distance = setInput.Distance
                    }
                });
            }

            exercises.Add(new PlannedExercise
            {
                Order = i + 1,
                Exercise = exercise,
                RestInterval = TimeSpan.FromSeconds(input.RestIntervalSeconds),
                AutoTriggerRestTimer = input.AutoTriggerRestTimer,
                TargetRepsLower = input.TargetRepsLower,
                TargetRepsUpper = input.TargetRepsUpper,
                TargetWeightIncrement = input.TargetWeightIncrement,
                PlannedSets = sets
            });
        }

        return (exercises, null);
    }

    [McpServerTool(Name = "create_workout_plan")]
    [Description("""
        Create a new workout plan. Call get_exercises first to get exercise IDs.
        plannedExercisesJson is a JSON array where each element has:
          exerciseId (guid, required),
          restIntervalSeconds (int, default 120),
          autoTriggerRestTimer (bool, default true),
          targetRepsLower (int, default 8),
          targetRepsUpper (int, default 12),
          targetWeightIncrement (decimal, default 2.5),
          sets: array of { setType ("Warm-up"|"Set"|"Drop-set"), reps (int?), weight (decimal?), time (decimal?), distance (decimal?) }
        Order and OrderForSetType are assigned automatically by position.
        NOTE: WorkoutStatistics/ExerciseStatistics/WorkoutPlanStatistics are not updated — the app recalculates them on next use.
        """)]
    public static async Task<string> CreateWorkoutPlan(
        GymDataService dataService,
        [Description("Display name for the workout plan")] string name,
        [Description("Whether the plan is active (default true)")] bool isActive = true,
        [Description("Whether this is a regular routine (default false)")] bool isRegularRoutine = false,
        [Description("JSON array of planned exercises — see tool description for schema")] string plannedExercisesJson = "[]")
    {
        try
        {
            var allExercises = await dataService.GetExercisesAsync();
            var (exercises, error) = ParsePlannedExercises(plannedExercisesJson, allExercises);
            if (error is not null)
                return JsonSerializer.Serialize(new { Success = false, Error = error });

            var plan = new WorkoutPlan
            {
                Name = name,
                IsAcitve = isActive,
                IsRegularRoutine = isRegularRoutine,
                PlannedExercises = exercises!
            };

            var plans = await dataService.GetWorkoutPlansAsync();
            plans.Add(plan);
            await dataService.SaveWorkoutPlansAsync(plans);

            return JsonSerializer.Serialize(new { Success = true, plan.Id, plan.Name }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }

    [McpServerTool(Name = "update_workout_plan")]
    [Description("""
        Full replace of an existing workout plan by ID. All fields including exercises are replaced.
        Call get_workout_plans first to get the plan ID and current structure.
        The plannedExercisesJson schema is identical to create_workout_plan.
        WARNING: This is a full replace — exercises not included will be removed. New GUIDs are assigned to exercises.
        """)]
    public static async Task<string> UpdateWorkoutPlan(
        GymDataService dataService,
        [Description("The GUID of the workout plan to update")] string planId,
        [Description("Display name for the workout plan")] string name,
        [Description("Whether the plan is active")] bool isActive = true,
        [Description("Whether this is a regular routine")] bool isRegularRoutine = false,
        [Description("JSON array of planned exercises — see create_workout_plan for schema")] string plannedExercisesJson = "[]")
    {
        try
        {
            if (!Guid.TryParse(planId, out var id))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid planId GUID: '{planId}'" });

            var plans = await dataService.GetWorkoutPlansAsync();
            var existing = plans.FirstOrDefault(p => p.Id == id);
            if (existing is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"WorkoutPlan with id '{id}' not found." });

            var allExercises = await dataService.GetExercisesAsync();
            var (exercises, error) = ParsePlannedExercises(plannedExercisesJson, allExercises);
            if (error is not null)
                return JsonSerializer.Serialize(new { Success = false, Error = error });

            var updated = existing with
            {
                Name = name,
                IsAcitve = isActive,
                IsRegularRoutine = isRegularRoutine,
                PlannedExercises = exercises!
            };

            var index = plans.IndexOf(existing);
            plans[index] = updated;
            await dataService.SaveWorkoutPlansAsync(plans);

            return JsonSerializer.Serialize(new { Success = true, updated.Id, updated.Name }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }

    [McpServerTool(Name = "delete_workout_plan")]
    [Description("""
        Delete a workout plan permanently by ID. Does not delete associated workout history.
        Call get_workout_plans first to confirm the plan ID and name.
        confirmName must exactly match the plan name (case-insensitive) to prevent accidental deletion.
        WARNING: WorkoutPlanStatistics for this plan will become orphaned.
        """)]
    public static async Task<string> DeleteWorkoutPlan(
        GymDataService dataService,
        [Description("The GUID of the workout plan to delete")] string planId,
        [Description("The exact name of the plan — must match to confirm deletion")] string confirmName)
    {
        try
        {
            if (!Guid.TryParse(planId, out var id))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid planId GUID: '{planId}'" });

            var plans = await dataService.GetWorkoutPlansAsync();
            var plan = plans.FirstOrDefault(p => p.Id == id);
            if (plan is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"WorkoutPlan with id '{id}' not found." });

            if (!string.Equals(plan.Name, confirmName, StringComparison.OrdinalIgnoreCase))
                return JsonSerializer.Serialize(new { Success = false, Error = $"confirmName '{confirmName}' does not match plan name '{plan.Name}'. Deletion cancelled." });

            plans.Remove(plan);
            await dataService.SaveWorkoutPlansAsync(plans);

            return JsonSerializer.Serialize(new { Success = true, DeletedId = plan.Id, DeletedName = plan.Name }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }
}
