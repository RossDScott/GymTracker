namespace GymTracker.Domain.Models;
public class DefaultData
{
    public static class SetType
    {
        public const string WarmUp = "Warm-up";
        public const string Set = "Set";
        public const string DropSet = "Drop-set";
    }

    public static TimeSpan DefaultRestInterval => TimeSpan.FromMinutes(2);
    public const decimal DefaultWeightIncrement = 1m;
    public const int DefaultTargetRepsLower = 4;
    public const int DefaultTargetRepsUpper = 6;

    public required string[] TargetBodyParts { get; set; }
    public required string[] Equipment { get; set; }

    public string[] SetTypes => new string[]
    {
        DefaultData.SetType.WarmUp,
        DefaultData.SetType.Set,
        DefaultData.SetType.DropSet
    };

    public required Exercise[] Exercises { get; set; }
}
