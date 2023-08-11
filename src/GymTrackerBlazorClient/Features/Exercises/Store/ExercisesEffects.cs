using Fluxor;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Extensions;
using GymTracker.Domain.Models;
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

        await dispatcher.DispatchWithDelay(new SetExerciseAction(exercise));
        await dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Exercises", "/exercises", false, Icons.Material.Filled.List),
            new BreadcrumbItem(exercise.Name, $"/exercises/{exercise.Id}", false, Icons.Material.Filled.Edit),
        }));
    }

    [EffectMethod]
    public async Task OnCreateNewExercise(CreateNewExerciseAction action, IDispatcher dispatcher)
    {
        var newExercise = new Exercise { Id = Guid.NewGuid(), MetricType = MetricType.Weight };

        await dispatcher.DispatchWithDelay(new SetExerciseAction(newExercise));
        await dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("Exercises", "/exercises", false, Icons.Material.Filled.List),
            new BreadcrumbItem("New", "/exercises/new", false, Icons.Material.Filled.Add),
        }));
    }

    [EffectMethod]
    public async Task OnAddOrUpdateExercise(AddOrUpdateExerciseAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.Exercise;
        var exercises = await _clientStorage.Exercises.GetOrDefaultAsync();
        var exercise = await _clientStorage.Exercises.FindOrDefaultByIdAsync(updateDTO.Id);

        var isNew = false;
        if(exercise is null)
        {
            exercise = new Exercise { Id = updateDTO.Id };
            exercises.Add(exercise);
            isNew = true;
        }
        
        exercise.Name = updateDTO.Name;
        exercise.MetricType = updateDTO.MetricType;
        exercise.BodyTarget = updateDTO.BodyTarget.ToArray();
        exercise.Equipment = updateDTO.Equipment.ToArray();
        exercise.IsAcitve = updateDTO.IsActive;

        await _clientStorage.Exercises.SetAsync(exercises);
        dispatcher.Dispatch(new SetExercisesAction(exercises));
        dispatcher.Dispatch(new SetExerciseAction(exercise));

        if(isNew)
            dispatcher.Dispatch(new NavigateToNewExerciseAction(exercise.Id));
    }
}
