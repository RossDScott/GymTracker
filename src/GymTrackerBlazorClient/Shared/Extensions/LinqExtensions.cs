namespace GymTracker.BlazorClient.Shared.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<(T item, int index)> LoopIndex<T>(this IEnumerable<T> self) 
        => self.Select((item, index) => (item, index));
}
