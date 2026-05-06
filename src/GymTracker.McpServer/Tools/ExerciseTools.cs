using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class ExerciseTools
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool(Name = "get_exercises")]
    [Description("""
        Get all tracked exercises, optionally filtered by body target or equipment.
        The MetricType field in each exercise indicates which metric fields to populate when recording sets:
          "Weight"          — reps (int) + weight (decimal, kg)
          "Reps"            — reps (int) only
          "Time"            — time (decimal, seconds) only
          "TimeAndDistance" — time (decimal, seconds) + distance (decimal, metres)
        """)]
    public static async Task<string> GetExercises(
        GymDataService dataService,
        [Description("Filter by body target (e.g. 'Chest', 'Back', 'Legs')")] string? bodyTarget = null,
        [Description("Filter by equipment (e.g. 'Barbell', 'Dumbbell', 'Cable')")] string? equipment = null,
        [Description("Include inactive exercises (default false)")] bool includeInactive = false)
    {
        var exercises = await dataService.GetExercisesAsync();

        if (!includeInactive)
            exercises = exercises.Where(e => e.IsAcitve).ToList();

        if (!string.IsNullOrEmpty(bodyTarget))
            exercises = exercises
                .Where(e => e.BodyTarget.Any(b => b.Contains(bodyTarget, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        if (!string.IsNullOrEmpty(equipment))
            exercises = exercises
                .Where(e => e.Equipment.Any(eq => eq.Contains(equipment, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        return JsonSerializer.Serialize(exercises.Select(e => new
        {
            e.Id,
            e.Name,
            MetricType = e.MetricType.ToString(),
            e.BodyTarget,
            e.Equipment
        }), JsonOptions);
    }
}
