using System.ComponentModel;
using System.Text.Json;
using GymTracker.Domain.Models;
using ModelContextProtocol.Server;

namespace GymTracker.McpServer.Tools;

[McpServerToolType]
public static class ExerciseWriteTools
{
    private static readonly JsonSerializerOptions OutputJsonOptions = new() { WriteIndented = true };

    [McpServerTool(Name = "create_exercise")]
    [Description("""
        Create a new exercise. Call get_exercises first to check for duplicates and see existing body target / equipment values.

        MetricType controls which metric fields are tracked when recording sets:
          "Weight"          — reps (int) + weight (decimal, kg). Use for barbell, dumbbell, machine exercises.
          "Reps"            — reps (int) only. Use for bodyweight exercises (pull-ups, push-ups, dips).
          "Time"            — time (decimal, seconds) only. Use for holds and static exercises (planks).
          "TimeAndDistance" — time (decimal, seconds) + distance (decimal, metres). Use for cardio (rowing, running).

        bodyTargets: comma-separated list of muscle groups, e.g. "Chest,Triceps" or "Back,Biceps".
        equipment: comma-separated list of equipment, e.g. "Barbell" or "Dumbbell,Cable".
        Use the same capitalisation and spelling as existing exercises (visible in get_exercises) for consistency.
        """)]
    public static async Task<string> CreateExercise(
        GymDataService dataService,
        [Description("Display name for the exercise")] string name,
        [Description("Metric tracked for this exercise: \"Weight\" (reps+kg), \"Reps\" (reps only), \"Time\" (seconds only), \"TimeAndDistance\" (seconds+metres)")] string metricType,
        [Description("Comma-separated muscle groups targeted, e.g. \"Chest,Triceps\" (optional)")] string bodyTargets = "",
        [Description("Comma-separated equipment used, e.g. \"Barbell\" or \"Dumbbell,Cable\" (optional)")] string equipment = "",
        [Description("Default rest interval in seconds between sets (default 120)")] int defaultRestIntervalSeconds = 120,
        [Description("Whether the exercise is active and visible in the app (default true)")] bool isActive = true,
        [Description("Show a progress chart for this exercise on the home screen (default false)")] bool showChartOnHomepage = false)
    {
        try
        {
            if (!Enum.TryParse<MetricType>(metricType, ignoreCase: true, out var parsedMetricType))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid metricType '{metricType}'. Valid values: Weight, Reps, Time, TimeAndDistance." });

            var exercises = await dataService.GetExercisesAsync();

            if (exercises.Any(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase)))
                return JsonSerializer.Serialize(new { Success = false, Error = $"An exercise named '{name}' already exists." });

            var exercise = new Exercise
            {
                Name = name,
                MetricType = parsedMetricType,
                BodyTarget = SplitCsv(bodyTargets),
                Equipment = SplitCsv(equipment),
                DefaultRestInterval = TimeSpan.FromSeconds(defaultRestIntervalSeconds),
                IsAcitve = isActive,
                ShowChartOnHomepage = showChartOnHomepage
            };

            exercises.Add(exercise);
            await dataService.SaveExercisesAsync(exercises);

            return JsonSerializer.Serialize(new { Success = true, exercise.Id, exercise.Name, MetricType = parsedMetricType.ToString() }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }

    [McpServerTool(Name = "update_exercise")]
    [Description("""
        Full replace of an existing exercise by ID. All fields are replaced.
        Call get_exercises first to get the exercise ID and current values.
        WARNING: Changing MetricType on an exercise that has workout history will make historical set data inconsistent.

        MetricType values:
          "Weight"          — reps (int) + weight (decimal, kg)
          "Reps"            — reps (int) only
          "Time"            — time (decimal, seconds) only
          "TimeAndDistance" — time (decimal, seconds) + distance (decimal, metres)

        bodyTargets: comma-separated list of muscle groups, e.g. "Chest,Triceps".
        equipment: comma-separated list of equipment, e.g. "Barbell" or "Dumbbell,Cable".
        """)]
    public static async Task<string> UpdateExercise(
        GymDataService dataService,
        [Description("The GUID of the exercise to update")] string exerciseId,
        [Description("Display name for the exercise")] string name,
        [Description("Metric tracked for this exercise: \"Weight\" (reps+kg), \"Reps\" (reps only), \"Time\" (seconds only), \"TimeAndDistance\" (seconds+metres)")] string metricType,
        [Description("Comma-separated muscle groups targeted, e.g. \"Chest,Triceps\" (optional)")] string bodyTargets = "",
        [Description("Comma-separated equipment used, e.g. \"Barbell\" or \"Dumbbell,Cable\" (optional)")] string equipment = "",
        [Description("Default rest interval in seconds between sets (default 120)")] int defaultRestIntervalSeconds = 120,
        [Description("Whether the exercise is active and visible in the app (default true)")] bool isActive = true,
        [Description("Show a progress chart for this exercise on the home screen (default false)")] bool showChartOnHomepage = false)
    {
        try
        {
            if (!Guid.TryParse(exerciseId, out var id))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid exerciseId GUID: '{exerciseId}'" });

            if (!Enum.TryParse<MetricType>(metricType, ignoreCase: true, out var parsedMetricType))
                return JsonSerializer.Serialize(new { Success = false, Error = $"Invalid metricType '{metricType}'. Valid values: Weight, Reps, Time, TimeAndDistance." });

            var exercises = await dataService.GetExercisesAsync();
            var existing = exercises.FirstOrDefault(e => e.Id == id);
            if (existing is null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"Exercise with id '{id}' not found." });

            var nameConflict = exercises.FirstOrDefault(e => e.Id != id && string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
            if (nameConflict is not null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"Another exercise named '{name}' already exists." });

            var updated = existing with
            {
                Name = name,
                MetricType = parsedMetricType,
                BodyTarget = SplitCsv(bodyTargets),
                Equipment = SplitCsv(equipment),
                DefaultRestInterval = TimeSpan.FromSeconds(defaultRestIntervalSeconds),
                IsAcitve = isActive,
                ShowChartOnHomepage = showChartOnHomepage
            };

            var index = exercises.IndexOf(existing);
            exercises[index] = updated;
            await dataService.SaveExercisesAsync(exercises);

            return JsonSerializer.Serialize(new { Success = true, updated.Id, updated.Name, MetricType = parsedMetricType.ToString() }, OutputJsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }

    private static string[] SplitCsv(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
