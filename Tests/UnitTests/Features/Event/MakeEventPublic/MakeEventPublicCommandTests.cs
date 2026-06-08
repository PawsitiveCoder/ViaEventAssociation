using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;

namespace UnitTests.Features.Event.MakeEventPublic;

[TestSubject(typeof(MakeEventPublicCommand))]
public class MakeEventPublicCommandTests
{
    [Fact]
    public void MakeEventPublicCommand_ValidInput_Success()
    {
        string id = Guid.NewGuid().ToString();

        var result = MakeEventPublicCommand.Create(id);
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(id, command.EventId.Value.ToString());
    }

    [Fact]
    public void MakeEventPublicCommand_InvalidId_Failure()
    {
        string invalidId = "not-a-guid";

        Action action = () => MakeEventPublicCommand.Create(invalidId);
        Assert.Throws<FormatException>(action);
    }
}
