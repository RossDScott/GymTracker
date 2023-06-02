using Fluxor;
using MudBlazor;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.AppBar.Store;

[FeatureState]
public record AppBarState
{
    public BreadcrumbItem HomeBreadcrumb => new BreadcrumbItem("Home", "/", false, Icons.Material.Filled.Home);

    public ImmutableList<BreadcrumbItem> Breadcrumbs { get; init; }

    private AppBarState() { 
        Breadcrumbs = ImmutableList.Create<BreadcrumbItem>(HomeBreadcrumb);
    }
}
