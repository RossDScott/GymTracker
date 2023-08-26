namespace GymTracker.Domain.Models.Extensions;
public static class IOrderableExtensions
{
    public static ICollection<T> ChangeOrder<T>(
        this ICollection<T> list,
        T target, OrderDirection direction) where T : IOrderable
    {
        if (direction == OrderDirection.Up && target.Order == 0)
            return list;
        if (direction == OrderDirection.Down && target.Order == list.Count - 1)
            return list;

        if (direction == OrderDirection.Up)
        {
            var itemAbove = list.Single(x => x.Order == target.Order - 1);
            target.Order--;
            itemAbove.Order++;
        }

        if (direction == OrderDirection.Down)
        {
            var itemBelow = list.Single(x => x.Order == target.Order + 1);
            target.Order++;
            itemBelow.Order--;
        }

        return list;
    }
}
