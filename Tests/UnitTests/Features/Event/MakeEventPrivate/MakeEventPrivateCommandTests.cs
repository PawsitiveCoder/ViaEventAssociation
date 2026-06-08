using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;

namespace UnitTests.Features.Event.MakeEventPrivate;

[TestSubject(typeof(MakeEventPrivateCommand))]
public class MakeEventPrivateCommandTests
{
    [Fact]
    public void MakeEventPrivateCommand_ValidInput_Success()
    {
        string id = Guid.NewGuid().ToString();

        var result = MakeEventPrivateCommand.Create(id);
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(id, command.EventId.Value.ToString());
    }

    [Fact]
    public void MakeEventPrivateCommand_InvalidId_Failure()
    {
        string invalidId = "not-a-guid";

        Action action = () => MakeEventPrivateCommand.Create(invalidId);
        Assert.Throws<FormatException>(action);
    }
}
