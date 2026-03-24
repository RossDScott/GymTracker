using System.Text.Json;
using GymTracker.Domain;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Statistics;

namespace GymTracker.McpServer;

public class GymDataService
{
    private readonly IDataBackupClient _backupClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GymDataService(IDataBackupClient backupClient)
    {
        _backupClient = backupClient;
    }

    public async Task<List<Exercise>> GetExercisesAsync()
    {
        var json = await _backupClient.DownloadBackupItem("Exercises");
        return JsonSerializer.Deserialize<List<Exercise>>(json, JsonOptions) ?? [];
    }

    public async Task<List<WorkoutPlan>> GetWorkoutPlansAsync()
    {
        var json = await _backupClient.DownloadBackupItem("WorkoutPlans");
        return JsonSerializer.Deserialize<List<WorkoutPlan>>(json, JsonOptions) ?? [];
    }

    public async Task<List<Workout>> GetWorkoutsAsync()
    {
        var json = await _backupClient.DownloadBackupItem("Workouts");
        return JsonSerializer.Deserialize<List<Workout>>(json, JsonOptions) ?? [];
    }

    public async Task<List<ExerciseStatistic>> GetExerciseStatisticsAsync()
    {
        var json = await _backupClient.DownloadBackupItem("ExerciseStatistics");
        return JsonSerializer.Deserialize<List<ExerciseStatistic>>(json, JsonOptions) ?? [];
    }

    public async Task<List<WorkoutStatistic>> GetWorkoutStatisticsAsync()
    {
        var json = await _backupClient.DownloadBackupItem("WorkoutStatistics");
        return JsonSerializer.Deserialize<List<WorkoutStatistic>>(json, JsonOptions) ?? [];
    }

    public async Task<List<WorkoutPlanStatistic>> GetWorkoutPlanStatisticsAsync()
    {
        var json = await _backupClient.DownloadBackupItem("WorkoutPlanStatistics");
        return JsonSerializer.Deserialize<List<WorkoutPlanStatistic>>(json, JsonOptions) ?? [];
    }
}
