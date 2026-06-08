using JetBrains.Annotations;
using UnitTests.Fakes;
using UnitTests.Mocks;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;

namespace UnitTests.Features.Event.SetMaxNumberOfGuests;

[TestSubject(typeof(SetMaxNumberOfGuestsHandler))]
public class SetMaxNumberOfGuestsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_SetsMaxGuests()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new SetMaxNumberOfGuestsHandler(repository);

        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var commandResult = SetMaxNumberOfGuestsCommand.Create(eventAggregate.Id.Value.ToString(), 40);
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var updatedEvent = await repository.GetByIdAsync(eventAggregate.Id.Value);
        Assert.Equal(40, updatedEvent!.MaxNumberOfGuests.Value);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new SetMaxNumberOfGuestsHandler(repository);

        var commandResult = SetMaxNumberOfGuestsCommand.Create(Guid.NewGuid().ToString(), 40);
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("SetMaxNumberOfGuestsHandler.HandleAsync", result.Error!.Code);
        Assert.Contains("not found", result.Error.Description);
    }
}
