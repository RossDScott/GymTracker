using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class WorkoutHistoryTools
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool(Name = "get_workout_history")]
    [Description("Get completed workout history with exercises, sets, reps, and weights. Returns most recent workouts first.")]
    public static async Task<string> GetWorkoutHistory(
        GymDataService dataService,
        [Description("Filter to workouts containing this exercise (partial match, e.g. 'Bench')")] string? exerciseName = null,
        [Description("Filter by workout plan name (partial match)")] string? planName = null,
        [Description("Filter to workouts from this date onwards (YYYY-MM-DD)")] string? fromDate = null,
        [Description("Filter to workouts up to this date (YYYY-MM-DD)")] string? toDate = null,
        [Description("Max number of workouts to return (default 20)")] int limit = 20)
    {
        var workouts = await dataService.GetWorkoutsAsync();

        if (!string.IsNullOrEmpty(fromDate) && DateTimeOffset.TryParse(fromDate, out var from))
            workouts = workouts.Where(w => w.WorkoutStart >= from).ToList();

        if (!string.IsNullOrEmpty(toDate) && DateTimeOffset.TryParse(toDate, out var to))
            workouts = workouts.Where(w => w.WorkoutStart <= to.AddDays(1)).ToList();

        if (!string.IsNullOrEmpty(planName))
            workouts = workouts
                .Where(w => w.Plan.Name.Contains(planName, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!string.IsNullOrEmpty(exerciseName))
            workouts = workouts
                .Where(w => w.Exercises.Any(e =>
                    e.Exercise.Name.Contains(exerciseName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        var result = workouts
            .OrderByDescending(w => w.WorkoutStart)
            .Take(limit)
            .Select(w => new
            {
                w.Id,
                Plan = w.Plan.Name,
                w.WorkoutStart,
                w.WorkoutEnd,
                w.Notes,
                Exercises = w.Exercises.OrderBy(e => e.Order).Select(e => new
                {
                    Exercise = e.Exercise.Name,
                    Sets = e.Sets.OrderBy(s => s.Order).Select(s => new
                    {
                        s.SetType,
                        s.Metrics.Reps,
                        s.Metrics.Weight,
                        s.Metrics.Time,
                        s.Metrics.Distance,
                        s.Completed
                    })
                })
            });

        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
