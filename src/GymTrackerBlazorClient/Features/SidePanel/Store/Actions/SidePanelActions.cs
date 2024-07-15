using GymTracker.BlazorClient.Features.SidePanel.Models;

namespace GymTracker.BlazorClient.Features.SidePanel.Store.Actions;

public record ShowSidePanelAction(Type Type, SidePanelReference SidePanelReference);
public record HideSidePanelAction();