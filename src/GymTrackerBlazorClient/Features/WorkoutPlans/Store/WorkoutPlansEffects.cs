using Fluxor;
using GymTracker.BlazorClient.Extensions;
using GymTracker.BlazorClient.Features.AppBar.Store;
using GymTracker.Domain.Models;
using GymTracker.Domain.Models.Extensions;
using GymTracker.LocalStorage.Core;
using MudBlazor;

namespace GymTracker.BlazorClient.Features.WorkoutPlans.Store;

public class WorkoutPlansEffects
{
    private readonly IClientStorage _clientStorage;

    public WorkoutPlansEffects(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    [EffectMethod]
    public async Task OnFetchWorkoutPlansInitialStateAction(FetchWorkoutPlansInitialStateAction action, IDispatcher dispatcher)
    {
        await LoadWorkoutPlans(dispatcher);

        dispatcher.Dispatch(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("WorkoutPlans", "/workout-plans", false, Icons.Material.Filled.List)
        }));
    }

    [EffectMethod]
    public async Task OnFetchWorkoutPlan(FetchWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.Id);

        dispatcher.DispatchWithDelay(new SetWorkoutPlanAction(workoutPlan));
        dispatcher.DispatchWithDelay(new SetBreadcrumbAction(new[]
        {
            new BreadcrumbItem("WorkoutPlans", "/workout-plans", false, Icons.Material.Filled.List),
            new BreadcrumbItem(workoutPlan.Name, $"/workout-plans", false, Icons.Material.Filled.Edit),
        }));
    }

    [EffectMethod]
    public async Task OnAddExerciseToWorkoutPlan(AddExerciseToWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = await _clientStorage.Exercises.FindByIdAsync(action.ExerciseId);
        var order = workoutPlan.PlannedExercises.Count();

        var plannedExercise = new PlannedExercise
        {
            Exercise = exercise,
            Order = order,
            RestInterval = exercise.DefaultRestInterval,
            TargetWeightIncrement = DefaultData.DefaultWeightIncrement,
            TargetRepsLower = DefaultData.DefaultTargetRepsLower,
            TargetRepsUpper = DefaultData.DefaultTargetRepsUpper,
            AutoTriggerRestTimer = true,
        };

        var plannedSets = order == 0
                ? new List<PlannedExerciseSet>
                    {
                        new PlannedExerciseSet{ Order = 0, OrderForSetType = 1, SetType = "Warm-up",
                            TargetMetrics = new ExerciseSetMetrics{ Reps = 10 }},
                        new PlannedExerciseSet{Order = 1, OrderForSetType = 2, SetType = "Warm-up",
                            TargetMetrics = new ExerciseSetMetrics{ Reps = 10 }}
                    }
                : new List<PlannedExerciseSet>();

        var setStartPos = order == 0 ? 2 : 0;
        var defaultSets = new List<PlannedExerciseSet>
        {
            new PlannedExerciseSet{ Order = setStartPos, OrderForSetType = 1, SetType = "Set",
                TargetMetrics = new ExerciseSetMetrics{ Reps = 8 }},
            new PlannedExerciseSet{ Order = setStartPos + 1, OrderForSetType = 2, SetType = "Set",
                TargetMetrics = new ExerciseSetMetrics{ Reps = 8 }},
            new PlannedExerciseSet{ Order = setStartPos + 2, OrderForSetType = 3, SetType = "Set",
                TargetMetrics = new ExerciseSetMetrics{ Reps = 8 }}
        };
        plannedSets.AddRange(defaultSets);
        plannedExercise.PlannedSets = plannedSets;
        workoutPlan.PlannedExercises.Add(plannedExercise);

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
    }

    [EffectMethod]
    public async Task OnUpdateExerciseForWorkoutPlan(UpdateExerciseForWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var dto = action.EditDTO;
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercises = workoutPlan.PlannedExercises.ToList();
        var plannedExerciseToUpdate = exercises.Single(x => x.Id == dto.Id);
        var sourceExercise = await _clientStorage.Exercises.FindByIdAsync(plannedExerciseToUpdate.Exercise.Id);

        plannedExerciseToUpdate.RestInterval = dto.RestInterval;
        plannedExerciseToUpdate.AutoTriggerRestTimer = dto.AutoTriggerRestTimer;
        plannedExerciseToUpdate.TargetRepsLower = dto.TargetRepsLower;
        plannedExerciseToUpdate.TargetRepsUpper = dto.TargetRepsUpper;
        plannedExerciseToUpdate.TargetWeightIncrement = dto.TargetWeightIncrement;
        plannedExerciseToUpdate.PlannedSets = dto.PlannedSets
                                                 .Select(x => x.ToModel())
                                                 .ToList()
                                                 .OrderSetTypes();
        plannedExerciseToUpdate.Exercise = sourceExercise;
        workoutPlan.PlannedExercises = exercises;

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetExerciseAction(plannedExerciseToUpdate));
    }

    [EffectMethod]
    public async Task OnUpsertWorkoutPlan(UpsertWorkoutPlanAction action, IDispatcher dispatcher)
    {
        var updateDTO = action.WorkoutPlan;

        var workoutPlan = await _clientStorage.WorkoutPlans.FindOrDefaultByIdAsync(updateDTO.Id)
            ?? new WorkoutPlan { Id = updateDTO.Id };

        workoutPlan.Name = updateDTO.Name;
        workoutPlan.IsAcitve = updateDTO.IsActive;
        workoutPlan.IsRegularRoutine = updateDTO.IsRegularRoutine;

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));

        await LoadWorkoutPlans(dispatcher);
    }

    [EffectMethod]
    public async Task OnFetchExercise(FetchExerciseAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = workoutPlan.PlannedExercises.Single(x => x.Id == action.ExerciseId);

        dispatcher.Dispatch(new SetExerciseAction(exercise));
    }

    [EffectMethod]
    public async Task OnChangeOrder(ChangeOrderAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercises = workoutPlan.PlannedExercises;
        var targetExercise = exercises.Single(x => x.Id == action.ExerciseId);

        exercises.ChangeOrder(targetExercise, action.Direction);

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
        dispatcher.DispatchWithDelay(new SetExerciseAction(targetExercise));
    }

    [EffectMethod]
    public async Task OnAddSetToExercise(AddSetToExerciseAction action, IDispatcher dispatcher)
    {
        var dto = action.ExerciseDetail;
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercises = workoutPlan.PlannedExercises;
        var exercise = exercises.Single(x => x.Id == dto.Id);
        var lastSet = exercise.PlannedSets.LastOrDefault() ?? new PlannedExerciseSet { SetType = "Warm-Up", TargetMetrics = new ExerciseSetMetrics() };

        var newSet = new PlannedExerciseSet
        {
            Order = exercise.PlannedSets.Count(),
            OrderForSetType = exercise.PlannedSets.Count(x => x.SetType == lastSet.SetType) + 1,
            SetType = lastSet.SetType,
            TargetMetrics = lastSet.TargetMetrics
        };
        exercise.PlannedSets.Add(newSet);

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetExerciseAction(exercise));
    }

    [EffectMethod]
    public async Task OnSetWorkoutPlanType(SetWorkoutPlanTypeAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindOrDefaultByIdAsync(action.WorkoutPlanId)
            ?? new WorkoutPlan { Id = action.WorkoutPlanId };

        workoutPlan.WorkoutType = action.WorkoutType;

        if (action.WorkoutType == WorkoutType.Circuit)
        {
            workoutPlan.PlannedExercises = new List<PlannedExercise>();
            workoutPlan.CircuitConfig ??= new CircuitConfig();
        }
        else
        {
            workoutPlan.PlannedExercises = new List<PlannedExercise>();
            workoutPlan.CircuitConfig = null;
        }

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
    }

    [EffectMethod]
    public async Task OnUpdateCircuitConfig(UpdateCircuitConfigAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        workoutPlan.CircuitConfig ??= new CircuitConfig();
        workoutPlan.CircuitConfig.Rounds = action.Rounds;
        workoutPlan.CircuitConfig.RestBetweenRounds = action.RestBetweenRounds;

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
    }

    [EffectMethod]
    public async Task OnAddExerciseToCircuit(AddExerciseToCircuitAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = await _clientStorage.Exercises.FindByIdAsync(action.ExerciseId);
        var order = workoutPlan.PlannedExercises.Count;

        var targetMetrics = exercise.MetricType is MetricType.Time or MetricType.TimeAndDistance
            ? new ExerciseSetMetrics { Time = 30m }
            : exercise.MetricType == MetricType.Weight
                ? new ExerciseSetMetrics { Reps = 10, Weight = 20m }
                : new ExerciseSetMetrics { Reps = 20 };

        var plannedExercise = new PlannedExercise
        {
            Exercise = exercise,
            Order = order,
            PlannedSets = new List<PlannedExerciseSet>
            {
                new PlannedExerciseSet
                {
                    Order = 0, OrderForSetType = 1, SetType = DefaultData.SetType.Set,
                    TargetMetrics = targetMetrics
                }
            }
        };

        workoutPlan.PlannedExercises.Add(plannedExercise);
        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetWorkoutPlanAction(workoutPlan));
    }

    [EffectMethod]
    public async Task OnFetchCircuitExercise(FetchCircuitExerciseAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = workoutPlan.PlannedExercises.Single(x => x.Id == action.ExerciseId);
        dispatcher.Dispatch(new SetCircuitExerciseAction(exercise));
    }

    [EffectMethod]
    public async Task OnUpdateCircuitExerciseTarget(UpdateCircuitExerciseTargetAction action, IDispatcher dispatcher)
    {
        var workoutPlan = await _clientStorage.WorkoutPlans.FindByIdAsync(action.WorkoutPlanId);
        var exercise = workoutPlan.PlannedExercises.Single(x => x.Id == action.ExerciseId);
        var targetSet = exercise.PlannedSets.OrderBy(s => s.Order).FirstOrDefault();

        if (targetSet != null)
        {
            targetSet.TargetMetrics = new ExerciseSetMetrics
            {
                Reps = action.TargetReps,
                Weight = action.TargetWeight,
                Time = action.TargetTime
            };
        }

        await _clientStorage.WorkoutPlans.UpsertAsync(workoutPlan);
        dispatcher.Dispatch(new SetCircuitExerciseAction(exercise));
    }

    private async Task LoadWorkoutPlans(IDispatcher dispatcher)
    {
        var workoutPlans = await _clientStorage.WorkoutPlans.GetOrDefaultAsync();
        dispatcher.Dispatch(new SetWorkoutPlansAction(workoutPlans));
    }
}
