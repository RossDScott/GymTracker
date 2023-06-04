using GymTracker.Domain.Models.ClientStorage;

namespace GymTracker.Domain.Configuration;
public interface IAppSettingsProvider
{
    ValueTask<AppSettings> GetAsync();
}
