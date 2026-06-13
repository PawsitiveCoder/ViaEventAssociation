using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.AppEntry.Dispatcher;
using ViaEventAssociation.Core.Domain.Common.UnitOfWork;

namespace ViaEventAssociation.Core.Application;

public static class CommandHandlersServiceExtensions
{
    public static IServiceCollection AddApplicationCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<CommandDispatcher>();
        services.AddScoped<ICommandDispatcher>(serviceProvider =>
        {
            var commandDispatcher = serviceProvider.GetRequiredService<CommandDispatcher>();
            var logger = serviceProvider.GetRequiredService<ILogger<TimingCommandDispatcher>>();
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            var timingDispatcher = new TimingCommandDispatcher(commandDispatcher, logger);
            return new TransactionalCommandDispatcher(timingDispatcher, unitOfWork);
        });

        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(IsCommandHandlerInterface)
                .Select(i => new { Service = i, Implementation = t }))
            .ToList()
            .ForEach(h => services.AddScoped(h.Service, h.Implementation));

        return services;
    }

    private static bool IsCommandHandlerInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommandHandler<>);
}
