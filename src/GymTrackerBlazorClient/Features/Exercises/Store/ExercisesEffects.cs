﻿using Fluxor;
using GymTracker.BlazorClient.Extensions;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.Exercises.Store;

public class ExercisesEffects
{
    private readonly IClientStorage _clientStorage;

    public ExercisesEffects(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchExercises(FetchExercisesAction action, IDispatcher dispatcher)
    {
        var settings = await _clientStorage.AppSettings.GetOrDefaultAsync();
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();

        dispatcher.Dispatch(new SetInitialDataAction(
            settings.TargetBodyParts,
            settings.Equipment,
            exercises));

        dispatcher.Dispatch(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Exercises", "/exercises", false, Icons.Material.Filled.List)
        }));
    }

    [EffectMethod]
    public async Task OnFetchExercise(FetchExerciseAction action, IDispatcher dispatcher)
    {
        var exercise = await _clientStorage.Exercises.FindByIdAsync(action.Id);

        dispatcher.DispatchWithDelay(new SetExerciseAction(exercise));
        dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Exercises", "/exercises", false, Icons.Material.Filled.List),
            new BreadcrumbItem(exercise.Name, $"/exercises", false, Icons.Material.Filled.Edit),
        }));
    }

    [EffectMethod]
    public Task OnCreateNewExercise(CreateNewExerciseAction action, IDispatcher dispatcher)
    {
        var newExercise = new Exercise { Id = Guid.NewGuid(), Name = "", MetricType = MetricType.Weight };

        dispatcher.DispatchWithDelay(new SetExerciseAction(newExercise));
        dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Exercises", "/exercises", false, Icons.Material.Filled.List),
            new BreadcrumbItem("New", "/exercises", false, Icons.Material.Filled.Add),
        }));

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task OnUpsertExercise(UpsertExerciseAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.Exercise;
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();
        var exercise = await _clientStorage.Exercises.FindOrDefaultByIdAsync(updateDTO.Id)
            ?? new Exercise { Id = updateDTO.Id, Name = updateDTO.Name };

        exercise.Name = updateDTO.Name;
        exercise.MetricType = updateDTO.MetricType;
        exercise.BodyTarget = updateDTO.BodyTarget.ToArray();
        exercise.Equipment = updateDTO.Equipment.ToArray();
        exercise.IsAcitve = updateDTO.IsActive;
        exercise.ShowChartOnHomepage = updateDTO.ShowChartOnHomepage;

        var response = await _clientStorage.Exercises.UpsertAsync(exercise);
        dispatcher.Dispatch(new SetExercisesAction(exercises));
        dispatcher.Dispatch(new SetExerciseAction(exercise));
    }
}
