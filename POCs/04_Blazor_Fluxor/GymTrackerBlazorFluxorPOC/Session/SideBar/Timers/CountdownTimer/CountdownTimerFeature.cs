using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session.SideBar.Timers.CountdownTimer;

public class CountdownTimerFeature : Feature<CountdownTimerState>
{
    public override string GetName() => "CountdownTimer";

    protected override CountdownTimerState GetInitialState()
    {
        return new CountdownTimerState();
    }
}