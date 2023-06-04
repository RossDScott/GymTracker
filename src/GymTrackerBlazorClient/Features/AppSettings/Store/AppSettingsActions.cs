using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public record FetchSettingsAction();
public record SetSettingsAction(Models.ClientStorage.AppSettings Settings);

public record BackupAllAction();
public record RestoreAllAction();
public record UpdateSettingsAction(Models.ClientStorage.AppSettings Settings);