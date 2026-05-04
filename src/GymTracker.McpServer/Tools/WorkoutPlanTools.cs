using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class WorkoutPlanTools
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool(Name = "get_workout_plans")]
    [Description("Get all workout plans with their exercises and target sets")]
    public static async Task<string> GetWorkoutPlans(
        GymDataService dataService,
        [Description("Include inactive plans (default false)")] bool includeInactive = false,
        [Description("Only show regular routine plans")] bool regularRoutineOnly = false)
    {
        var plans = await dataService.GetWorkoutPlansAsync();

        if (!includeInactive)
            plans = plans.Where(p => p.IsAcitve).ToList();

        if (regularRoutineOnly)
            plans = plans.Where(p => p.IsRegularRoutine).ToList();

        var result = plans.Select(p => new
        {
            p.Id,
            p.Name,
            p.IsRegularRoutine,
            WorkoutType = p.WorkoutType.ToString(),
            CircuitConfig = p.CircuitConfig == null ? null : new
            {
                p.CircuitConfig.Rounds,
                RestBetweenRoundsSeconds = (int)p.CircuitConfig.RestBetweenRounds.TotalSeconds
            },
            Exercises = p.PlannedExercises.OrderBy(e => e.Order).Select(e => new
            {
                Exercise = e.Exercise.Name,
                e.Exercise.MetricType,
                e.TargetRepsLower,
                e.TargetRepsUpper,
                e.TargetWeightIncrement,
                Sets = e.PlannedSets.OrderBy(s => s.Order).Select(s => new
                {
                    s.SetType,
                    s.TargetMetrics.Reps,
                    s.TargetMetrics.Weight,
                    s.TargetMetrics.Time,
                    s.TargetMetrics.Distance
                })
            })
        });

        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
