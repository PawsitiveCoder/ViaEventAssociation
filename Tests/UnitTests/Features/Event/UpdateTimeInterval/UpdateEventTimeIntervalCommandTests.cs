using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;

namespace UnitTests.Features.Event.UpdateTimeInterval;

[TestSubject(typeof(UpdateEventTimeIntervalCommand))]
public class UpdateEventTimeIntervalCommandTests
{
    [Fact]
    public void UpdateEventTimeIntervalCommand_ValidInput_Success()
    {
        string id = Guid.NewGuid().ToString();
        var currentTime = new DateTime(2023, 8, 20, 10, 0, 0);
        var startDateTime = new DateTime(2023, 8, 25, 10, 0, 0);
        var endDateTime = new DateTime(2023, 8, 25, 14, 0, 0);

        var result = UpdateEventTimeIntervalCommand.Create(id, startDateTime, endDateTime, currentTime);
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(id, command.EventId.Value.ToString());
        Assert.Equal(startDateTime, command.StartDateTime);
        Assert.Equal(endDateTime, command.EndDateTime);
    }

    [Fact]
    public void UpdateEventTimeIntervalCommand_InvalidInterval_Failure()
    {
        string id = Guid.NewGuid().ToString();
        var currentTime = new DateTime(2023, 8, 20, 10, 0, 0);
        var startDateTime = new DateTime(2023, 8, 25, 10, 0, 0);
        var endDateTime = new DateTime(2023, 8, 25, 10, 30, 0); // Less than 1 hour duration

        var result = UpdateEventTimeIntervalCommand.Create(id, startDateTime, endDateTime, currentTime);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("at least 1 hour", result.Error.Description);
    }
}
