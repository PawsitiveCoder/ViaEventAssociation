using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.AppEntry;

namespace UnitTests.Common.Dispatcher;

class ServiceBuilder
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    public static ServiceBuilder Init() => new();

    public ServiceBuilder WithHandler<TCommand, THandler>() where THandler : class, ICommandHandler<TCommand>
    {
        _serviceCollection.AddScoped<ICommandHandler<TCommand>, THandler>();
        return this;
    }

    public ServiceProvider Build() => _serviceCollection.BuildServiceProvider();
}
