﻿using ApexCharts;
using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain.Models.Extensions;
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
    public ImmutableArray<DateTimeOffset> Dates { get; init; } = ImmutableArray<DateTimeOffset>.Empty;
    public string[] ChartDateXAxisLabels => Dates.Select(s => s.ToString("dd/MM")).ToArray();
    public ImmutableArray<Exercise> FilteredExercises { get; init; } = ImmutableArray<Exercise>.Empty;
    public ImmutableArray<Models.Workout> Workouts { get; init; } = ImmutableArray<Models.Workout>.Empty;
    public int PageSize { get; init; } = 5;
    public int PageCount => (int)Math.Ceiling((decimal)Dates.Length / PageSize);
    public int SelectedPage { get; init; } = 1;

    public ImmutableArray<DateTimeOffset> FilteredAndPagedDates
        => Dates
            .Skip(PageSize * (SelectedPage - 1))
            .Take(PageSize)
            .ToImmutableArray();

    public string GetTotalVolumeForDate(DateTimeOffset date)
    {
        return Workouts
                .Where(x => x.Plan.Id == SelectedWorkoutPlanId)
                .First(x => x.WorkoutEnd!.Value == date)
                .GetWeightTotalVolumeWithMeasure();
    }
}

public record Exercise
{
    public required string ExerciseName { get; init; }
    public required ImmutableArray<string> SetNames { get; init; }
    public required ImmutableArray<ExerciseRecord> Records { get; set; }

    public ApexChartOptions<ExerciseRecord> ChartOptions
        => new()
        {
            Chart = new Chart
            {
                Background = "#32333d",
                Toolbar = new Toolbar
                {
                    Show = false
                },
                ForeColor = "#fff",
                Width = "100%",
                Height = "200px"
            },
            Colors = new List<string> { "#77B6EA" },
        };
}

public record ExerciseRecord
{
    public required DateTimeOffset Date { get; init; }
    public required ImmutableArray<Set> Sets { get; init; }
    public required string TotalVolumeWithMeasure { get; init; }
    public decimal TotalVolume { get; init; }
}

public record Set
{
    public required string SetName { get; init; }
    public required string Measure { get; init; }
}

