using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Fakes;

public class FakeCommand { }

public class FakeHandler : ICommandHandler<FakeCommand>
{
    public Task<Result> HandleAsync(FakeCommand command) => Result.Success();
}
