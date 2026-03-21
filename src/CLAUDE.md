# CLAUDE.md

## Version Bumping

After making code changes, increment the **patch** segment of the `<Version>` in `GymTrackerBlazorClient/GymTracker.BlazorClient.csproj`.

The version format is `Major.Minor.Patch.Revision` (e.g. `1.5.8.0`). Bump the third segment (patch) by 1 and reset revision to 0.

## Target Devices

- **Primary (tablet):** Samsung Galaxy Tab A7 SM-T500 — 10.4" display, 2000 x 1200 pixels (WUXGA+), ~224 PPI
- **Mobile:** Samsung Galaxy S25 — 6.2" display, 2340 x 1080 pixels (FHD+), ~416 PPI

## Branching
When on main branch always create a new branch
After the changes have been applied ask if you want to push and create a PR