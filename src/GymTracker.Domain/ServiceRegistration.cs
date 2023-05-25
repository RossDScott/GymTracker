using GymTracker.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain;
public static class ServiceRegistration
{
    public static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<DefaultDataBuilderService>();
    }
}
