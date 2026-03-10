namespace GymTracker.BlazorClient.Shared;

public class ErrorService
{
    public Exception? CurrentException { get; private set; }
    public event Action? OnErrorChanged;

    public void CaptureException(Exception ex)
    {
        CurrentException = ex;
        OnErrorChanged?.Invoke();
    }

    public void ClearException()
    {
        CurrentException = null;
        OnErrorChanged?.Invoke();
    }
}
