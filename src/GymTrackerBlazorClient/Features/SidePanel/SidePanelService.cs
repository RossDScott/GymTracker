using FluentValidation.TestHelper;
using Fluxor;
using GymTracker.BlazorClient.Features.SidePanel.Models;
using GymTracker.BlazorClient.Features.SidePanel.Store.Actions;
using Microsoft.AspNetCore.Components;

namespace GymTracker.BlazorClient.Features.SidePanel;

public class SidePanelService
{
    private readonly IDispatcher _dispatcher;

    public SidePanelService(IDispatcher dispatcher)
    {
        this._dispatcher = dispatcher;
    }

    public SidePanelReference<TResult> ShowSidePanel<TComponent, TResult>() 
        where TResult : class
        where TComponent : IComponent
    {
        var child = new SidePanelReference(_dispatcher);
        var response = new SidePanelReference<TResult>(child);
        _dispatcher.Dispatch(new ShowSidePanelAction(typeof(TComponent), child));
        return response;
    }
        
}
