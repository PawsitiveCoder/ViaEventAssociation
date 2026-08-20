using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

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

        var result = MakeEventPrivateCommand.Create(invalidId);

        Assert.True(result.HasErrors);
        Assert.Equal(ErrorType.Validation, result.Error?.ErrorType);
    }
}
