# Fluxor Upgrade Plan: v5.9.0 → v6.9.0

## Overview

"Fluzor" refers to **Fluxor** — the Redux/Flux state management library for Blazor used throughout this project.

---

## Current State

### Current Versions

| Package | Current Version | Project File |
|---|---|---|
| `Fluxor` | 5.9.0 | `GymTracker.Domain.csproj` |
| `Fluxor.Blazor.Web` | 5.9.0 | `GymTracker.BlazorClient.csproj` |
| `Fluxor.Blazor.Web.ReduxDevTools` | 5.9.0 | `GymTracker.BlazorClient.csproj` |

### Affected Files

**Project files (package references):**
- `src/GymTracker.Domain/GymTracker.Domain.csproj`
- `src/GymTrackerBlazorClient/GymTracker.BlazorClient.csproj`

**Source files using Fluxor APIs: 41 C# files + 22 Razor components**
(No source-level changes required — see Breaking Changes section below)

**Not in scope:**
- `POCs/04_Blazor_Fluxor/` — historical proof-of-concept, not part of the main solution

---

## Target Version

**Fluxor 6.9.0** (released November 24, 2025) — latest stable release.

All three packages must be updated together:
- `Fluxor` → 6.9.0
- `Fluxor.Blazor.Web` → 6.9.0
- `Fluxor.Blazor.Web.ReduxDevTools` → 6.9.0

---

## Breaking Changes Assessment

### 1. `FluxorComponent` now uses `IAsyncDisposable` instead of `IDisposable`

Components overriding `Dispose(bool disposing)` must switch to `DisposeAsyncCore(bool disposing)`.

**Impact:** None. No component in the main project overrides `Dispose(bool)`.

### 2. `UseReduxDevTools()` requires explicit `using` directive

`using Fluxor.Blazor.Web.ReduxDevTools;` must be added to any file calling `UseReduxDevTools()`.

**Impact:** None. `UseReduxDevTools()` is not currently called in `Program.cs`.

### 3. `UseReduxDevTools()` no longer implicitly calls `UseRouting()`

Both must now be called explicitly if both are desired.

**Impact:** None. `UseReduxDevTools()` is not called.

### 4. ReduxDevTools switched from Newtonsoft.Json to System.Text.Json

State/action types with `[JsonProperty]` attributes need updating to `[JsonPropertyName]`.

**Impact:** None. All state types are plain C# records with no JSON attributes.

### 5. Dropped support for .NET < 8

**Impact:** None. Project already targets `net10.0`.

---

## Step-by-Step Upgrade Instructions

### Step 1: Update `GymTracker.Domain.csproj`

File: `src/GymTracker.Domain/GymTracker.Domain.csproj`

```xml
<!-- Before -->
<PackageReference Include="Fluxor" Version="5.9.0" />

<!-- After -->
<PackageReference Include="Fluxor" Version="6.9.0" />
```

### Step 2: Update `GymTracker.BlazorClient.csproj`

File: `src/GymTrackerBlazorClient/GymTracker.BlazorClient.csproj`

```xml
<!-- Before -->
<PackageReference Include="Fluxor.Blazor.Web" Version="5.9.0" />
<PackageReference Include="Fluxor.Blazor.Web.ReduxDevTools" Version="5.9.0" />

<!-- After -->
<PackageReference Include="Fluxor.Blazor.Web" Version="6.9.0" />
<PackageReference Include="Fluxor.Blazor.Web.ReduxDevTools" Version="6.9.0" />
```

### Step 3: (Optional) Enable Redux DevTools

If Redux DevTools support is desired in development, update `Program.cs`:

```csharp
using Fluxor.Blazor.Web.ReduxDevTools; // Add this using directive

builder.Services.AddFluxor(o =>
{
    o.ScanAssemblies(typeof(Program).Assembly);
#if DEBUG
    o.UseReduxDevTools();
    // Note: UseRouting() must also be called explicitly if needed (no longer implied)
#endif
});
```

### Step 4: Clean and restore

```bash
cd src
dotnet clean GymTrackerBlazorClient.sln
dotnet restore GymTrackerBlazorClient.sln
```

Cleaning is important — stale `bin/`/`obj/` artifacts can cause type-resolution errors.

### Step 5: Build

```bash
dotnet build GymTrackerBlazorClient.sln
```

No compilation errors are expected given the breaking change assessment above.

---

## Testing Strategy

Since there is no automated test suite, testing is manual.

### Smoke Test (build validation)

- [ ] `dotnet build` succeeds with no errors
- [ ] Application starts without console errors related to Fluxor/store initialisation
- [ ] `<Fluxor.Blazor.Web.StoreInitializer />` in `App.razor` renders without error

### Feature-by-Feature Manual Tests

| Feature | What to verify |
|---|---|
| Home page | `HomeState` loads; charts and workout history render |
| Exercises | List renders; filter controls work; add/edit updates state |
| Workout Plans | Create/edit/delete plans; add and reorder exercises |
| Workout (Perform) | Stopwatch ticks; countdown timer counts down and resets; exercise navigation updates `ExerciseDetailState`; history side panel shows data |
| End Workout | Completion flow fires `EndWorkoutEffects` correctly |
| App Settings | Settings load and save via `AppSettingsEffects` |
| Side Panel | Opens/closes; exercise picker resolves `SidePanelReference` task |
| Redux DevTools (if enabled) | Actions appear in browser DevTools timeline; no console errors |

### Regression Indicators

- Browser console errors containing "Fluxor", "store", or "could not resolve type"
- State not updating after actions are dispatched
- Components using `@inherits FluxorComponent` not re-rendering on state changes
- Timer effects (Stopwatch, CountdownTimer) not ticking

---

## Summary

| File | Change |
|---|---|
| `GymTracker.Domain.csproj` | `Fluxor` 5.9.0 → 6.9.0 |
| `GymTracker.BlazorClient.csproj` | `Fluxor.Blazor.Web` + `Fluxor.Blazor.Web.ReduxDevTools` 5.9.0 → 6.9.0 |
| `Program.cs` | Optional: enable Redux DevTools explicitly |
| All other files | No changes required |

**Risk level: Low.** Only two `.csproj` version bumps are mandatory.

---

## Sources

- [NuGet - Fluxor.Blazor.Web](https://www.nuget.org/packages/Fluxor.Blazor.Web/)
- [GitHub - mrpmorris/Fluxor](https://github.com/mrpmorris/Fluxor)
- [Fluxor Release Notes](https://github.com/mrpmorris/Fluxor/blob/master/Docs/releases.md)
