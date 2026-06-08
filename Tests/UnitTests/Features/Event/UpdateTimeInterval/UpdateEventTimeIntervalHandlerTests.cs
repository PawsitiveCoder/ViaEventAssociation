using JetBrains.Annotations;
using UnitTests.Fakes;
using UnitTests.Mocks;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;

namespace UnitTests.Features.Event.UpdateTimeInterval;

[TestSubject(typeof(UpdateEventTimeIntervalHandler))]
public class UpdateEventTimeIntervalHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_UpdatesTimeInterval()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventTimeIntervalHandler(repository);

        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var currentTime = new DateTime(2023, 8, 20, 10, 0, 0);
        var startDateTime = new DateTime(2023, 8, 25, 10, 0, 0);
        var endDateTime = new DateTime(2023, 8, 25, 14, 0, 0);

        var commandResult = UpdateEventTimeIntervalCommand.Create(eventAggregate.Id.Value.ToString(), startDateTime, endDateTime, currentTime);
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var updatedEvent = await repository.GetByIdAsync(eventAggregate.Id.Value);
        Assert.NotNull(updatedEvent!.TimeInterval);
        Assert.Equal(startDateTime, updatedEvent.TimeInterval.StartDateTime);
        Assert.Equal(endDateTime, updatedEvent.TimeInterval.EndDateTime);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventTimeIntervalHandler(repository);

        var currentTime = new DateTime(2023, 8, 20, 10, 0, 0);
        var startDateTime = new DateTime(2023, 8, 25, 10, 0, 0);
        var endDateTime = new DateTime(2023, 8, 25, 14, 0, 0);

        var commandResult = UpdateEventTimeIntervalCommand.Create(Guid.NewGuid().ToString(), startDateTime, endDateTime, currentTime);
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("UpdateEventTimeIntervalHandler.HandleAsync", result.Error!.Code);
        Assert.Contains("not found", result.Error.Description);
    }
}
