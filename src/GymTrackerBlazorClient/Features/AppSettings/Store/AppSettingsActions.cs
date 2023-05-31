using Models = GymTracker.Domain.Models;

namespace GymTracker.BlazorClient.Features.AppSettings.Store;

public record FetchSettingsAction();
public record SetSettingsAction(Models.AppSettings Settings);

public record BackupAllAction();
public record RestoreAllAction();
public record UpdateSettingsAction(Models.AppSettings Settings);