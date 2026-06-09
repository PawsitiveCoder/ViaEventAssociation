using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Tools.OperationResult;

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

        var result = UpdateEventTimeIntervalCommand.Create(eventAggregate.Id.Value.ToString(), startDateTime, endDateTime, currentTime);
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = Assert.Single(repository.Events);
        Assert.NotNull(updatedEvent.TimeInterval);
        Assert.Equal(command.StartDateTime, updatedEvent.TimeInterval.StartDateTime);
        Assert.Equal(command.EndDateTime, updatedEvent.TimeInterval.EndDateTime);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventTimeIntervalHandler(repository);
        var currentTime = new DateTime(2023, 8, 20, 10, 0, 0);
        var startDateTime = new DateTime(2023, 8, 25, 10, 0, 0);
        var endDateTime = new DateTime(2023, 8, 25, 14, 0, 0);

        var result = UpdateEventTimeIntervalCommand.Create(Guid.NewGuid().ToString(), startDateTime, endDateTime, currentTime);
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.HasErrors);
        var error = Assert.Single(operationResult.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }
}
