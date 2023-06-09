using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public static class ExercisesReducers
{
    [ReducerMethod]
    public static ExercisesState OnSetInitialData(ExercisesState state, SetInitialDataAction action)
    {
        var bodyTargets = action.Exercises
                        .SelectMany(x => x.BodyTarget)
                        .Distinct()
                        .OrderBy(x => x)
                        .Select(x => new CheckItem(x, false))
                        .ToImmutableList();

        var response = state with
        {
            TargetBodyParts = action.TargetBodyParts.OrderBy(x => x).ToImmutableArray(),
            Equipment = action.Equipment.OrderBy(x => x).ToImmutableArray(),
            OriginalList = action.Exercises.ToImmutableArray(),
            Exercises = action.Exercises
                .OrderBy(x => x.Name)
                .Select(ToListItem)
                .ToImmutableArray(),
            SelectedExercise = null,
            Filter = new ExercisesFilter
            {
                ActiveOption = ActiveFilterOption.Active,
                SearchTerm = string.Empty,
                BodyTargets = bodyTargets,
                Results = action.Exercises
                                .Filter(
                                    ActiveFilterOption.Active,
                                    string.Empty,
                                    bodyTargets)
                                .ToImmutableArray()
            }
        };

        return response;
    }

    [ReducerMethod]
    public static ExercisesState OnSetExercises(ExercisesState state, SetExercisesAction action) =>
        state with
        {
            OriginalList = action.Exercises.ToImmutableArray(),
            Exercises = action.Exercises
                .OrderBy(x => x.Name)
                .Select(ToListItem)
                .ToImmutableArray()
        };

    [ReducerMethod]
    public static ExercisesState OnSetExercise(ExercisesState state, SetExerciseAction action) =>
        state with { SelectedExercise = action.Exercise.ToDetailItem() };

    [ReducerMethod]
    public static ExercisesState OnAddOrUpdateExercise(ExercisesState state, AddOrUpdateExerciseAction action) =>
        state with
        {
            SelectedExercise = state.SelectedExercise! with
            {
                Name = action.Exercise.Name,
                MetricType = action.Exercise.MetricType,
                BodyTarget = action.Exercise.BodyTarget.ToImmutableArray(),
                IsActive = action.Exercise.IsActive
            }
        };

    [ReducerMethod]
    public static ExercisesState OnCreateNewExercise(ExercisesState state, CreateNewExerciseAction action) =>
        state with { SelectedExercise = new DetailItem { Id = Guid.NewGuid(), MetricType = MetricType.Weight } };

    [ReducerMethod]
    public static ExercisesState OnFilterByIsActiveAction(ExercisesState state, FilterByIsActiveAction action) =>
        state with
        {
            Filter = state.Filter with
            {
                ActiveOption = action.ActiveOption,
                Results = state.OriginalList
                    .Filter(
                        action.ActiveOption,
                        state.Filter.SearchTerm, 
                        state.Filter.BodyTargets)
                    .ToImmutableArray(),
            }
        };

    [ReducerMethod]
    public static ExercisesState OnFilterBySearchTermAction(ExercisesState state, FilterBySearchTermAction action) =>
        state with
        {
            Filter = state.Filter with
            {
                SearchTerm = action.SearchTerm,
                Results = state.OriginalList
                    .Filter(
                        state.Filter.ActiveOption,
                        action.SearchTerm,
                        state.Filter.BodyTargets)
                    .ToImmutableArray(),
            }
        };

    [ReducerMethod]
    public static ExercisesState OnToggleFilterByBodyTarget(ExercisesState state, ToggleFilterByBodyTargetAction action)
    {
        var existing = action.BodyTargetItem;
        var newTargetItem = existing with { IsChecked = !existing.IsChecked };
        var newList = state.Filter.BodyTargets.Replace(existing, newTargetItem);

        return state with
        {
            Filter = state.Filter with
            {
                BodyTargets = newList,
                Results = state.OriginalList
                .Filter(
                    state.Filter.ActiveOption,
                    state.Filter.SearchTerm, 
                    newList)
                .ToImmutableArray()
            }
        };
    }

    private static IEnumerable<ListItem> Filter(
        this IEnumerable<Exercise> exerciseList,
        ActiveFilterOption activeOption,
        string searchTerm,
        IEnumerable<CheckItem> bodyTargets)
    {
        var selectedBodyTargets = bodyTargets
            .Where(bt => bt.IsChecked)
            .Select(x => x.Name)
            .ToList();

        var hasBodyTargetsFilter =
            selectedBodyTargets.Count > 0 &&
            selectedBodyTargets.Count != bodyTargets.Count();

        return exerciseList
            .Where(x => activeOption switch
            {
                ActiveFilterOption.Active => x.IsAcitve,
                ActiveFilterOption.Inactive => !x.IsAcitve,
                _ => true
            })
            .Where(x => searchTerm is null || x.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))
            .Where(x => !hasBodyTargetsFilter || x.BodyTarget.Intersect(selectedBodyTargets).Any())
            .Select(ToListItem);
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
            IsActive = exercise.IsAcitve
        };
}
