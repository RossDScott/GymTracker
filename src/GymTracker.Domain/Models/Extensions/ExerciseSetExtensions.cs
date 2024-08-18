namespace GymTracker.Domain.Models.Extensions;
public static class ExerciseSetExtensions
{
    public static string ToSetTypeAndSequence(this WorkoutExerciseSet set)
        => $"{set.SetType} {set.OrderForSetType}";
}
