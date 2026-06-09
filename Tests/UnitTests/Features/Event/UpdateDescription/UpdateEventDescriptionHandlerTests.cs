using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.UpdateDescription;

[TestSubject(typeof(UpdateEventDescriptionHandler))]
public class UpdateEventDescriptionHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_UpdatesEventDescription()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventDescriptionHandler(repository);
        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var result = UpdateEventDescriptionCommand.Create(eventAggregate.Id.Value.ToString(), "New description");
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = Assert.Single(repository.Events);
        Assert.Equal(command.EventDescription, updatedEvent.Description);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventDescriptionHandler(repository);

        var result = UpdateEventDescriptionCommand.Create(Guid.NewGuid().ToString(), "New description");
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.HasErrors);
        var error = Assert.Single(operationResult.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }
}
