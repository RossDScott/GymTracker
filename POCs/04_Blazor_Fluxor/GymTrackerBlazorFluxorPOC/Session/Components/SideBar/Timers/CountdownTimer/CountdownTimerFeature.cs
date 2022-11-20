using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session.Components.SideBar.Timers.CountdownTimer;

public class CountdownTimerFeature : Feature<CountdownTimerState>
{
    public override string GetName() => "CountdownTimer";

    protected override CountdownTimerState GetInitialState()
    {
        return new CountdownTimerState();
    }
}