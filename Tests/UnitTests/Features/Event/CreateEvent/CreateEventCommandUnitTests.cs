
using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.CreateEvent;

[TestSubject(typeof(CreateEventCommand))]
public class CreateEventCommandUnitTests
{
    [Fact]
    public void CreateEventCommand_Empty_Success()
    {
        var result = CreateEventCommand.Create();
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.NotNull(command.Id.Value.ToString());
        Assert.NotEmpty(command.Id.Value.ToString());
    }
}
