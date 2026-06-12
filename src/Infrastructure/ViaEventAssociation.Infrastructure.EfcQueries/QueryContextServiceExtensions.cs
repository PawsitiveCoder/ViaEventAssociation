using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;

namespace ViaEventAssociation.Infrastructure.EfcQueries;

public static class QueryContextServiceExtensions
{
    public static IServiceCollection AddInfrastructureQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Select(i => new { Service = i, Implementation = t }))
            .ToList()
            .ForEach(h => services.AddScoped(h.Service, h.Implementation));

        return services;
    }
}
