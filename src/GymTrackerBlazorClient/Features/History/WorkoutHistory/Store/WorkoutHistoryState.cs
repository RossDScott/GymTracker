using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using MudBlazor;
using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.History.WorkoutHistory.Store;

[FeatureState]
public record WorkoutHistoryState
{
    public bool Initalised { get; init; } = false;
    public Guid SelectedWorkoutPlanId { get; init; }
    public DateRange WorkoutDateRange { get; init; } = new DateRange(DateTime.Now.AddMonths(-1), DateTime.Now);
    public ImmutableArray<ListItem> WorkoutPlans { get; init; } = ImmutableArray<ListItem>.Empty;
    public ImmutableArray<DateOnly> Dates { get; init; } = ImmutableArray<DateOnly>.Empty;
    public ImmutableArray<Exercise> FilteredExercises { get; init; } = ImmutableArray<Exercise>.Empty;
    public ImmutableArray<Models.Workout> Workouts { get; init; } = ImmutableArray<Models.Workout>.Empty;
    public int PageSize { get; init; } = 5;
    public int PageCount => (int)Math.Ceiling((decimal)Dates.Length / PageSize);
    public int SelectedPage { get; init; } = 1;

    public ImmutableArray<DateOnly> FilteredAndPagedDates
        => Dates
            .Skip(PageSize * (SelectedPage - 1))
            .Take(PageSize)
            .ToImmutableArray();
}

public record Exercise
{
    public required string ExerciseName { get; init; }
    public required ImmutableArray<string> SetNames { get; init; }
    public required ImmutableArray<ExerciseRecord> Records { get; set; }
}

public record ExerciseRecord
{
    public required DateOnly Date { get; init; }
    public required ImmutableArray<Set> Sets { get; init; }
    public required string TotalVolume { get; set; }
}

public record Set
{
    public required string SetName { get; init; }
    public required string Measure { get; init; }
}

