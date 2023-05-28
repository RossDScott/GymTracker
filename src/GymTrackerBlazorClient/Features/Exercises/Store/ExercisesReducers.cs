using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public static class ExercisesReducers
{
    [ReducerMethod]
    public static ExercisesState OnSetExercises(ExercisesState state, SetExercisesAction action) =>
        state with
        {
            OriginalList = action.Exercises.ToImmutableArray(),
            Exercises = action.Exercises
                .OrderBy(x => x.Name)
                .Select(ToListItem)
                .ToImmutableArray(),
            SelectedExercise = state.SelectedExercise is not null
                ? action.Exercises
                        .SingleOrDefault(x => x.Id == state.SelectedExercise.Id)?
                        .ToDetailItem()
                : null,
            Filter = state.Filter.BuildResults(action.Exercises)
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
    public static ExercisesState OnFilter(ExercisesState state, FilterAction action) =>
        state with
        {
            Filter = new ExercisesFilter
            {
                AvailableBodyTargets = state.Filter.AvailableBodyTargets,
                FilterOptions = action.FilterOptions,
            }.BuildResults(state.OriginalList)
        };
    

    private static ExercisesFilter BuildResults(this ExercisesFilter filterOptions, IEnumerable<Exercise> exerciseList) =>
        filterOptions with
        {
            Results = exerciseList
                        .Where(x => x.Name.Contains(filterOptions.FilterOptions.SearchTerm, StringComparison.InvariantCultureIgnoreCase))
                        .Select(ToListItem)
                        .ToImmutableArray()
        };
    
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
