using Fluxor;

namespace GymTracker.BlazorClient.Extensions;
public static class IDispatcherExtensions
{
    public static async Task DispatchWithDelay(this IDispatcher dispatcher, object action, int millisecondsDelay = 1)
    {
        await Task.Delay(millisecondsDelay);
        dispatcher.Dispatch(action);
    }

}
