using Fluxor;

namespace GymTrackerBlazorFluxorPOC.Session;

public class SessionFeature : Feature<SessionState>
{
    public override string GetName() => "Session";

    protected override SessionState GetInitialState()
    {
        return new SessionState();
    }
}