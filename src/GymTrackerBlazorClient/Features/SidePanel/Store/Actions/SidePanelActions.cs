using GymTracker.BlazorClient.Features.SidePanel.Models;
using Microsoft.AspNetCore.Components;

namespace GymTracker.BlazorClient.Features.SidePanel.Store.Actions;

public record ShowSidePanelAction(Type Type, SidePanelReference SidePanelReference);
public record HideSidePanelAction();