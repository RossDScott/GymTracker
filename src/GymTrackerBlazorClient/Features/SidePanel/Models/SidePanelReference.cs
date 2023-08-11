using Fluxor;
using GymTracker.BlazorClient.Features.SidePanel.Store.Actions;

namespace GymTracker.BlazorClient.Features.SidePanel.Models;

public class SidePanelReference
{
    private readonly TaskCompletionSource<SidePanelResponse> _taskCompletionSource;
    private readonly IDispatcher _dispatcher;

    public SidePanelReference(IDispatcher dispatcher)
    {
        _taskCompletionSource = new TaskCompletionSource<SidePanelResponse>();
        this._dispatcher = dispatcher;
    }

    public void SetResult(object data)
    {
        _taskCompletionSource.SetResult(SidePanelResponse.Ok(data));
        _dispatcher.Dispatch(new HideSidePanelAction());
    }
    public void Cancel() =>
        _taskCompletionSource.SetResult(SidePanelResponse.Cancel());

    public Task<SidePanelResponse> Task => _taskCompletionSource.Task;

    public async Task<SidePanelResponse<T>> ResultAs<T>() where T : class
    {
        var result = await Task;
        return new SidePanelResponse<T>(result.Data as T, result.Canceled);
    }
}

public class SidePanelReference<T> where T : class
{
    private readonly SidePanelReference _childRef;

    public SidePanelReference(SidePanelReference childRef)
    {
        _childRef = childRef;
    }

    public async Task<SidePanelResponse<T>> Result() => await _childRef.ResultAs<T>();
}

public class SidePanelResponse
{
    public object? Data { get; }
    public bool Canceled { get; }

    public SidePanelResponse(object? data, bool canceled)
    {
        Data = data;
        Canceled = canceled;
    }

    public static SidePanelResponse Ok(object data) => new(data, false);
    public static SidePanelResponse Cancel() => new(default, true);
}

public class SidePanelResponse<T>
{
    public T? Data { get; }
    public bool Canceled { get; }

    public SidePanelResponse(T? data, bool canceled)
    {
        Data = data;
        Canceled = canceled;
    }
}