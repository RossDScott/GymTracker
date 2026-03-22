namespace GymTracker.Domain.Models.Statistics;

public enum MilestoneType
{
    WeightPR,
    MaxRepsAtWeight,
    MaxReps,
    MaxTime,
    MaxDistance,
    MaxVolume
}

public record SetMilestone(MilestoneType Type, string Description);

public record ExerciseMilestones(
    Guid ExerciseId,
    string ExerciseName,
    IReadOnlyList<SetMilestone> SetMilestones,
    SetMilestone? VolumeMilestone);
