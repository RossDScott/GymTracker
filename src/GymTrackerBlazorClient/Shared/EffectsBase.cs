namespace GymTracker.BlazorClient.Shared;

public abstract class EffectsBase
{
    private readonly ErrorService _errorService;

    protected EffectsBase(ErrorService errorService)
    {
        _errorService = errorService;
    }

    protected async Task SafeHandle(Func<Task> action)
    {
        try { await action(); }
        catch (Exception ex) { _errorService.CaptureException(ex); }
    }
}
