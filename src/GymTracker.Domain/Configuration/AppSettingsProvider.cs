using GymTracker.Domain.Abstractions.Services.ClientStorage;
using GymTracker.Domain.Models.ClientStorage;

namespace GymTracker.Domain.Configuration;
public class AppSettingsProvider : IAppSettingsProvider
{
    private readonly IClientStorage _clientStorage;

    public AppSettingsProvider(IClientStorage clientStorage)
    {
        _clientStorage = clientStorage;
    }

    public ValueTask<AppSettings> GetAsync() => _clientStorage.AppSettings.GetOrDefaultAsync();
}
