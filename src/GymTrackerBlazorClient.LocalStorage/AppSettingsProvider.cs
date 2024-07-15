using GymTracker.Domain;
using GymTracker.Domain.Models;
using GymTracker.LocalStorage.Core;

namespace GymTracker.LocalStorage;
public class AppSettingsProvider : IAppSettingsProvider
{
    private readonly IClientStorage _clientStorage;

    public AppSettingsProvider(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    public ValueTask<AppSettings> GetAsync() => _clientStorage.AppSettings.GetOrDefaultAsync();
}
