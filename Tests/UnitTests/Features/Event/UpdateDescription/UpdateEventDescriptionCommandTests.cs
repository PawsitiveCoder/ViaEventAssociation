using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateDescription;

[TestSubject(typeof(UpdateEventDescriptionCommand))]
public class UpdateEventDescriptionCommandTests
{
    [Fact]
    public void UpdateEventDescriptionCommand_ValidInput_Success()
    {
        string id = Guid.NewGuid().ToString();
        string description = "Test description that is valid";

        var result = UpdateEventDescriptionCommand.Create(id, description);
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(id, command.EventId.Value.ToString());
        Assert.Equal(description, command.EventDescription.Value);
    }

    [Fact]
    public void UpdateEventDescriptionCommand_TooLongDescription_Failure()
    {
        string id = Guid.NewGuid().ToString();
        string longDescription = new string('a', 251);

        var result = UpdateEventDescriptionCommand.Create(id, longDescription);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("cannot exceed 250", result.Error.Description);
    }
}
