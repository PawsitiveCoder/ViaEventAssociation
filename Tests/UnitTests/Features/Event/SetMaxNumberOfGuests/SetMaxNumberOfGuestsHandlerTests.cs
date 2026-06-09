using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.SetMaxNumberOfGuests;

[TestSubject(typeof(SetMaxNumberOfGuestsHandler))]
public class SetMaxNumberOfGuestsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_SetsMaxNumberOfGuests()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new SetMaxNumberOfGuestsHandler(repository);
        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var result = SetMaxNumberOfGuestsCommand.Create(eventAggregate.Id.Value.ToString(), 40);
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = Assert.Single(repository.Events);
        Assert.Equal(command.MaxNumberOfGuests, updatedEvent.MaxNumberOfGuests);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new SetMaxNumberOfGuestsHandler(repository);

        var result = SetMaxNumberOfGuestsCommand.Create(Guid.NewGuid().ToString(), 40);
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.HasErrors);
        var error = Assert.Single(operationResult.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }
}
