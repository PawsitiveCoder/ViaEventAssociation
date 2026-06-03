using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Mocks;

public class MockCommandHandler<TCommand> : ICommandHandler<TCommand>
{
    public int InvokeCount { get; private set; } = 0;
    public bool WasInvoked => InvokeCount > 0;

    public TCommand? Command { get; private set; }

    public Task<Result> HandleAsync(TCommand command)
    {
        InvokeCount++;
        Command = command;
        return Result.Success();
    }
}
