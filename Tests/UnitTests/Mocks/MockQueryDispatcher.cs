using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;

namespace UnitTests.Mocks;

public class MockQueryDispatcher : IQueryDispatcher
{
    private readonly object? _answer;
    private readonly Exception? _exception;

    public MockQueryDispatcher(object? answer = null, Exception? exception = null)
    {
        _answer = answer;
        _exception = exception;
    }

    public Task<TAnswer> DispatchAsync<TQuery, TAnswer>(TQuery query)
        where TQuery : IQuery<TAnswer>
    {
        if (_exception is not null) throw _exception;

        return Task.FromResult(_answer is null ? default! : (TAnswer)_answer);
    }
}
