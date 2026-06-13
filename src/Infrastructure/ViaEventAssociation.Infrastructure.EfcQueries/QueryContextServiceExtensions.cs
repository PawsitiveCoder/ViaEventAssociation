using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;

namespace ViaEventAssociation.Infrastructure.EfcQueries;

public static class QueryContextServiceExtensions
{
    public static IServiceCollection AddInfrastructureQueryHandlers(this IServiceCollection services)
    {
        services.AddDbContext<QueryContext>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(IsQueryHandlerInterface)
                .Select(i => new { Service = i, Implementation = t }))
            .ToList()
            .ForEach(h => services.AddScoped(h.Service, h.Implementation));

        return services;
    }

    private static bool IsQueryHandlerInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);
}
