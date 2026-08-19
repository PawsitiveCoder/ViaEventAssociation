using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry.Dispatcher;

internal class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Type serviceType = typeof(ICommandHandler<TCommand>);
        var service = _serviceProvider.GetService(serviceType);

        if (service is null)
        {
            throw new InvalidOperationException($"No handler found for command: {typeof(TCommand)}.");
        }

        ICommandHandler<TCommand> handler = (ICommandHandler<TCommand>)service;
        return handler.HandleAsync(command);
    }
}
