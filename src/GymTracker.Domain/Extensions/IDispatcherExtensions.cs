using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Extensions;
public static class IDispatcherExtensions
{
    public static async Task DispatchWithDelay(this IDispatcher dispatcher, object action, int millisecondsDelay = 1)
    {
        await Task.Delay(millisecondsDelay);
        dispatcher.Dispatch(action);
    }
        
}
