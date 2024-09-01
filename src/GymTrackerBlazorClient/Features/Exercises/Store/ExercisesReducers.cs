using Fluxor;
using GymTracker.BlazorClient.Features.Common;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public static class ExercisesReducers
{
    [ReducerMethod]
    public static ExercisesState OnSetInitialData(ExercisesState state, SetInitialDataAction action)
    {
        var targetBodyParts = action.TargetBodyParts
                        .OrderBy(x => x)
                        .ToImmutableArray();

        var initialState = new ExercisesState
        {
            TargetBodyParts = targetBodyParts,
            Equipment = action.Equipment.OrderBy(x => x).ToImmutableArray(),
            OriginalList = action.Exercises.OrderBy(x => x.Name).ToImmutableArray(),
            Filter = new ExercisesFilter
            {
                ActiveOption = ActiveFilterOption.Active,
                SearchTerm = string.Empty,
                BodyTargets = targetBodyParts.Select(x => new CheckItem(x, false)).ToImmutableList(),
            },
            SelectedExercise = null
        }.FilterExercises();

        return initialState;
    }

    [ReducerMethod]
    public static ExercisesState OnSetExercises(ExercisesState state, SetExercisesAction action) =>
        FilterExercises(state with
        { OriginalList = action.Exercises.ToImmutableArray() });

    [ReducerMethod]
    public static ExercisesState OnSetExercise(ExercisesState state, SetExerciseAction action) =>
        state with
        {
            SelectedExercise = action.Exercise.ToDetailItem()
        };

    [ReducerMethod]
    public static ExercisesState OnCreateNewExercise(ExercisesState state, CreateNewExerciseAction action) =>
        state with
        {
            SelectedExercise = new DetailItem { Id = Guid.NewGuid(), MetricType = MetricType.Weight }
        };

    [ReducerMethod]
    public static ExercisesState OnUpdateFilter(ExercisesState state, UpdateFilterAction action) =>
        FilterExercises(state with
        { Filter = action.Filter });

    private static ExercisesState FilterExercises(this ExercisesState state)
    {
        var filter = state.Filter;
        var selectedBodyTargets = filter.BodyTargets
            .Where(bt => bt.IsChecked)
            .Select(x => x.Name)
            .ToList();

        var hasBodyTargetsFilter =
            selectedBodyTargets.Count > 0 &&
            selectedBodyTargets.Count != filter.BodyTargets.Count();

        return state with
        {
            Exercises = state.OriginalList
                .Where(x => filter.ActiveOption switch
                {
                    ActiveFilterOption.Active => x.IsAcitve,
                    ActiveFilterOption.Inactive => !x.IsAcitve,
                    _ => true
                })
                .Where(x => filter.SearchTerm is null || x.Name.Contains(filter.SearchTerm, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !hasBodyTargetsFilter || x.BodyTarget.Intersect(selectedBodyTargets).Any())
                .Select(ToListItem)
                .ToImmutableArray()
        };
    }

    private static ListItem ToListItem(this Exercise exercise) =>
        new ListItem(exercise.Id, exercise.Name, exercise.IsAcitve);

    private static DetailItem ToDetailItem(this Exercise exercise) =>
        new DetailItem
        {
            Id = exercise.Id,
            Name = exercise.Name,
            MetricType = exercise.MetricType,
            BodyTarget = exercise.BodyTarget.ToImmutableArray(),
            Equipment = exercise.Equipment.ToImmutableArray(),
            IsActive = exercise.IsAcitve,
            ShowChartOnHomepage = exercise.ShowChartOnHomepage,
        };
}
