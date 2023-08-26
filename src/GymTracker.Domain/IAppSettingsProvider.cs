using GymTracker.Domain.Models;

namespace GymTracker.Domain;
public interface IAppSettingsProvider
{
    ValueTask<AppSettings> GetAsync();
}
