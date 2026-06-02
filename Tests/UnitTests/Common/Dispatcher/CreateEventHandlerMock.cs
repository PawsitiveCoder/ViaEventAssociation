using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

public class CreateEventHandlerMock : ICommandHandler<CreateEventCommand>
{
    public int invokeCount = 0;
    public bool wasInvoked => invokeCount > 0;

    public CreateEventCommand CreateEventCommand { get; private set; } = null!;

    public Task<Result> HandleAsync(CreateEventCommand command)
    {
        invokeCount++;
        CreateEventCommand = command;
        return Result.Success();
    }
}
