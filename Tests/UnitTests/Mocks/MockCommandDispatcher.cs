using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Mocks;

public class MockCommandDispatcher : ICommandDispatcher
{
    private readonly Result _resultToReturn;
    public int InvokeCount { get; private set; } = 0;
    public bool WasInvoked => InvokeCount > 0;

    public MockCommandDispatcher(Result? resultToReturn = null) => _resultToReturn = resultToReturn ?? new Result();

    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        InvokeCount++;
        return _resultToReturn;
    }
}
