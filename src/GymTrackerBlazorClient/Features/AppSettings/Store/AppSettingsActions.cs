using System.Collections.Immutable;
using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public record FetchSettingsAction();
public record SetSettingsAction(Models.AppSettings Settings);

public record BackupAllAction();
public record RestoreAllAction();
public record UpdateAzureBlobBackupContainerSASURIAction(string URI);

public record UpdateTargetBodyAction(ImmutableArray<string> TargetBodyParts);
public record UpdateEquipmentAction(ImmutableArray<string> Equipment);
public record UpdateSetTypesAction(ImmutableArray<string> SetTypes);




//public record SetTargetBodyAction(IEnumerable<string> TargetBodyParts);
//public record AddTargetBodyAction(string TargetBody);
//public record DeleteTargetBodyAction(string TargetBody);
//public record SetEquipmentAction(IEnumerable<string> Equipment);
//public record AddEquipmentAction(string equipment);
//public record DeleteEquipmentAction(string equipment);