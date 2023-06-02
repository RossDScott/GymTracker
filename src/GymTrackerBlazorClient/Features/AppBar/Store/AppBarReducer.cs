using Fluxor;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.AppBar.Store;

public static class AppBarReducer
{
    [ReducerMethod]
    public static AppBarState OnResetBreadcrumbs(AppBarState state, ResetBreadcrumbsToHomeAction action) =>
        state with { Breadcrumbs = ImmutableList.Create(state.HomeBreadcrumb) };

    [ReducerMethod]
    public static AppBarState OnSetBreadcrumbs(AppBarState state, SetBreadcrumbAction action) =>
        state with { Breadcrumbs = new[] 
            { state.HomeBreadcrumb }
            .Concat(action.Breadcrumbs)
            .ToImmutableList() };
}
