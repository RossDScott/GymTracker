using GymTracker.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Domain;
public static class ServiceRegistration
{
    public static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<DefaultDataBuilderService>();
    }
}
