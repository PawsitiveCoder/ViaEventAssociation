using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.AppEntry;

namespace ViaEventAssociation.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCommandHandlers(
        this IServiceCollection services)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Select(i => new { Service = i, Implementation = t }))
                .ToList()
                .ForEach(h => services.AddScoped(h.Service, h.Implementation));

        return services;
    }
}
