using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session.Main;

public class SessionFeature : Feature<SessionState>
{
    public override string GetName() => "Session";

    protected override SessionState GetInitialState()
    {
        return new SessionState();
    }
}