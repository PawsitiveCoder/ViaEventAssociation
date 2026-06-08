using JetBrains.Annotations;
using UnitTests.Fakes;
using UnitTests.Mocks;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateDescription;

[TestSubject(typeof(UpdateEventDescriptionHandler))]
public class UpdateEventDescriptionHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_UpdatesDescription()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventDescriptionHandler(repository);

        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var commandResult = UpdateEventDescriptionCommand.Create(eventAggregate.Id.Value.ToString(), "New description");
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var updatedEvent = await repository.GetByIdAsync(eventAggregate.Id.Value);
        Assert.Equal("New description", updatedEvent!.Description.Value);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventDescriptionHandler(repository);

        var commandResult = UpdateEventDescriptionCommand.Create(Guid.NewGuid().ToString(), "New description");
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("UpdateEventDescriptionHandler.HandleAsync", result.Error!.Code);
        Assert.Contains("not found", result.Error.Description);
    }
}
