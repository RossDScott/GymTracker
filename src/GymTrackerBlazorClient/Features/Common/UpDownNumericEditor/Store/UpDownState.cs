namespace GymTracker.BlazorClient.Features.Common.UpDownNumericEditor.Store;

public record UpDownState
{
    public static decimal[] AvailableIncrements => new[] { 0.25M, 1M, 5M };
}