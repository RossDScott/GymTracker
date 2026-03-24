using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class ExerciseStatisticsTools
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool(Name = "get_exercise_statistics")]
    [Description("Get performance statistics and training logs for exercises. Useful for tracking progress and trends over time.")]
    public static async Task<string> GetExerciseStatistics(
        GymDataService dataService,
        [Description("Filter by exercise name (partial match, e.g. 'Bench Press')")] string? exerciseName = null,
        [Description("Only include logs from this date onwards (YYYY-MM-DD)")] string? fromDate = null,
        [Description("Max number of exercises to return (default 10)")] int limit = 10)
    {
        var stats = await dataService.GetExerciseStatisticsAsync();

        if (!string.IsNullOrEmpty(exerciseName))
            stats = stats
                .Where(s => s.ExerciseName.Contains(exerciseName, StringComparison.OrdinalIgnoreCase))
                .ToList();

        DateTimeOffset? fromDateParsed = null;
        if (!string.IsNullOrEmpty(fromDate) && DateTimeOffset.TryParse(fromDate, out var fd))
            fromDateParsed = fd;

        var result = stats.Take(limit).Select(s => new
        {
            s.ExerciseId,
            s.ExerciseName,
            s.ExerciseMetric,
            Logs = (fromDateParsed.HasValue
                    ? s.Logs.Where(l => l.WorkoutDateTime >= fromDateParsed.Value)
                    : s.Logs)
                .OrderByDescending(l => l.WorkoutDateTime)
                .Select(l => new
                {
                    l.WorkoutDateTime,
                    l.TotalVolume,
                    Sets = l.Sets.Select(set => new
                    {
                        set.Reps,
                        set.Weight,
                        set.Time,
                        set.Distance
                    })
                })
        });

        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
