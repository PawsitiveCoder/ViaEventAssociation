using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Domain.Common.Time;

namespace ViaEventAssociation.Infrastructure.Time;

public static class SystemTimeServiceExtensions
{
    public static IServiceCollection AddInfrastructureSystemTime(this IServiceCollection services)
    {
        services.AddSingleton<ISystemTime, SystemTime>();
        return services;
    }
}
