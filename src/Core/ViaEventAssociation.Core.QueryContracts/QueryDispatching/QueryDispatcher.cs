using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.QueryDispatching;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TAnswer> DispatchAsync<TQuery, TAnswer>(TQuery query)
        where TQuery : IQuery<TAnswer>
    {
        Type serviceType = typeof(IQueryHandler<TQuery, TAnswer>);
        var service = _serviceProvider.GetService(serviceType);

        if (service is null)
        {
            throw new InvalidOperationException($"No handler found for query: {typeof(TQuery)}.");
        }

        IQueryHandler<TQuery, TAnswer> handler = (IQueryHandler<TQuery, TAnswer>)service;
        return handler.HandleAsync(query);
    }
}
