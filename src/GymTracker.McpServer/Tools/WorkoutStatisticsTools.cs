using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class WorkoutStatisticsTools
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool(Name = "get_workout_statistics")]
    [Description("Get workout summaries including duration, total volume, total reps, and personal records")]
    public static async Task<string> GetWorkoutStatistics(
        GymDataService dataService,
        [Description("Filter by workout plan name (partial match)")] string? planName = null,
        [Description("Filter to workouts from this date onwards (YYYY-MM-DD)")] string? fromDate = null,
        [Description("Filter to workouts up to this date (YYYY-MM-DD)")] string? toDate = null,
        [Description("Max number of workouts to return (default 20)")] int limit = 20)
    {
        var stats = await dataService.GetWorkoutStatisticsAsync();

        if (!string.IsNullOrEmpty(planName))
            stats = stats
                .Where(s => s.WorkoutPlanName.Contains(planName, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!string.IsNullOrEmpty(fromDate) && DateTimeOffset.TryParse(fromDate, out var from))
            stats = stats.Where(s => s.CompletedOn >= from).ToList();

        if (!string.IsNullOrEmpty(toDate) && DateTimeOffset.TryParse(toDate, out var to))
            stats = stats.Where(s => s.CompletedOn <= to.AddDays(1)).ToList();

        var result = stats
            .OrderByDescending(s => s.CompletedOn)
            .Take(limit)
            .Select(s => new
            {
                s.WorkoutId,
                s.WorkoutPlanName,
                s.CompletedOn,
                TotalTime = s.TotalTime.ToString(@"h\:mm\:ss"),
                s.TotalWeightVolumeWithMeasure,
                s.TotalReps,
                s.HasVolumePR,
                Exercises = s.Exercises.Select(e => new
                {
                    e.ExerciseName,
                    MetricType = e.MetricType.ToString(),
                    MaxSet = e.MaxSet != null ? new
                    {
                        e.MaxSet.Reps,
                        e.MaxSet.Weight,
                        e.MaxSet.Time,
                        e.MaxSet.Distance
                    } : null,
                    e.AllCompleted,
                    e.HasPR
                })
            });

        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "get_workout_plan_statistics")]
    [Description("Get per-plan statistics including previous workout details and 6-month best volume")]
    public static async Task<string> GetWorkoutPlanStatistics(GymDataService dataService)
    {
        var stats = await dataService.GetWorkoutPlanStatisticsAsync();

        var result = stats.Select(s => new
        {
            s.WorkoutPlanId,
            s.BestWeightTotalVolumeIn6Months,
            PreviousWorkout = new
            {
                s.PreviousWorkout.WorkoutPlanName,
                s.PreviousWorkout.CompletedOn,
                TotalTime = s.PreviousWorkout.TotalTime.ToString(@"h\:mm\:ss"),
                s.PreviousWorkout.TotalWeightVolumeWithMeasure,
                s.PreviousWorkout.TotalReps
            }
        });

        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
