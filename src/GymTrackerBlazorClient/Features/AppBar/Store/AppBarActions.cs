using MudBlazor;

namespace GymTracker.BlazorClient.Features.AppBar.Store;

public record ResetBreadcrumbsToHomeAction();
public record SetBreadcrumbAction(IEnumerable<BreadcrumbItem> Breadcrumbs);
