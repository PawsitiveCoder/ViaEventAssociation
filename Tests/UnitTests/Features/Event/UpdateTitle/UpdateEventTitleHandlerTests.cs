using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.UpdateTitle;

[TestSubject(typeof(UpdateEventTitleHandler))]
public class UpdateEventTitleHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_UpdatesEventTitle()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventTitleHandler(repository);
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Draft);
        await repository.AddAsync(eventAggregate);

        var result = UpdateEventTitleCommand.Create(eventAggregate.Id.Value.ToString(), "Test title");
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = Assert.Single(repository.Events);
        Assert.Equal(command.EventTitle, updatedEvent.Title);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventTitleHandler(repository);

        var result = UpdateEventTitleCommand.Create(Guid.NewGuid().ToString(), "Test title");
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.HasErrors);
        var error = Assert.Single(operationResult.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }
}
