using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Domain.Common.Repository;
using ViaEventAssociation.Core.Domain.Common.UnitOfWork;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public static class DmContextServiceExtensions
{
    public static IServiceCollection AddInfrastructureDmContext(this IServiceCollection services)
    {
        services.AddDbContext<DmContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(IsRepositoryInterface)
                .Select(i => new { Service = i, Implementation = t }))
            .ToList()
            .ForEach(h => services.AddScoped(h.Service, h.Implementation));

        return services;
    }

    private static bool IsRepositoryInterface(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGenericRepository<,>))
            return true;

        return type.GetInterfaces().Any(IsRepositoryInterface);
    }
}
