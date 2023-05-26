using Fluxor;
using GymTracker.Domain.Models;
using System.Collections.Immutable;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public static class ExercisesReducers
{
    [ReducerMethod]
    public static ExercisesState OnSetExercises(ExercisesState state, SetExercisesAction action) =>
        state with
        {
            Exercises = action.Exercises
                .OrderBy(x => x.Name)
                .Select(ToListItem)
                .ToImmutableList(),
            SelectedExercise = state.SelectedExercise is not null
                ? action.Exercises
                        .Single(x => x.Id == state.SelectedExercise.Id)
                        .ToDetailItem()
                : null
        };

    [ReducerMethod]
    public static ExercisesState OnSetExercise(ExercisesState state, SetExerciseAction action) =>
        state with { SelectedExercise = action.Exercise.ToDetailItem() };

    [ReducerMethod]
    public static ExercisesState OnUpdateExercise(ExercisesState state, UpdateExerciseAction action) =>
        state with
        {
            SelectedExercise = state.SelectedExercise! with
            {
                Name = action.Exercise.Name,
                MetricType = action.Exercise.MetricType,
                BodyTarget = action.Exercise.BodyTarget.ToImmutableArray()
            }
        };

    private static ListItem ToListItem(this Exercise exercise) =>
        new ListItem(exercise.Id, exercise.Name);

    private static DetailItem ToDetailItem(this Exercise exercise) =>
        new DetailItem
        {
            Id = exercise.Id,
            Name = exercise.Name,
            MetricType = exercise.MetricType,
            BodyTarget = exercise.BodyTarget.ToImmutableArray()
        };
}
