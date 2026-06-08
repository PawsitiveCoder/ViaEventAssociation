using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;

namespace UnitTests.Features.Event.SetMaxNumberOfGuests;

[TestSubject(typeof(SetMaxNumberOfGuestsCommand))]
public class SetMaxNumberOfGuestsCommandTests
{
    [Fact]
    public void SetMaxNumberOfGuestsCommand_ValidInput_Success()
    {
        string id = Guid.NewGuid().ToString();
        int maxGuests = 30;

        var result = SetMaxNumberOfGuestsCommand.Create(id, maxGuests);
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(id, command.EventId.Value.ToString());
        Assert.Equal(maxGuests, command.MaxNumberOfGuests.Value);
    }

    [Fact]
    public void SetMaxNumberOfGuestsCommand_InvalidGuests_Failure()
    {
        string id = Guid.NewGuid().ToString();
        int invalidGuests = 100; // Assuming MaxNumberOfGuests has validation for max 50

        var result = SetMaxNumberOfGuestsCommand.Create(id, invalidGuests);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
}
