using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public record FetchSettingsAction();
public record SetSettingsAction(
    Models.ClientStorage.AppSettings Settings,
    IEnumerable<string> TargetBodyParts,
    IEnumerable<string> Equipment);

public record BackupAllAction();
public record RestoreAllAction();
public record UpdateSettingsAction(Models.ClientStorage.AppSettings Settings);

public record SetTargetBodyAction(IEnumerable<string> TargetBodyParts);
public record AddTargetBodyAction(string TargetBody);
public record DeleteTargetBodyAction(string TargetBody);
public record SetEquipmentAction(IEnumerable<string> Equipment);
public record AddEquipmentAction(string equipment);
public record DeleteEquipmentAction(string equipment);