using Fluxor;

namespace GymTracker.BlazorClient.Extensions;
public static class IDispatcherExtensions
{
    public static void DispatchWithDelay(this IDispatcher dispatcher, object action, int millisecondsDelay = 1)
    {
        Task.Run(async () =>
        {
            await Task.Delay(millisecondsDelay);
            dispatcher.Dispatch(action);
        });
    }
}