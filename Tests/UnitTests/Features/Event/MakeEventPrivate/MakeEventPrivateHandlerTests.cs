using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.MakeEventPrivate;

[TestSubject(typeof(MakeEventPrivateHandler))]
public class MakeEventPrivateHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_MarksAsPrivate()
    {
        var repository = new FakeEventAggregateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new MakeEventPrivateHandler(repository, unitOfWork);

        var eventAggregate = FakeEventAggregateFactory.Create();
        // Assume event is made public first to test if it goes private
        eventAggregate.MarkAsPublic();
        await repository.AddAsync(eventAggregate);

        var commandResult = MakeEventPrivateCommand.Create(eventAggregate.Id.Value.ToString());
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var updatedEvent = await repository.GetByIdAsync(eventAggregate.Id.Value);
        Assert.Equal(EventVisibility.Private, updatedEvent!.Visibility);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new MakeEventPrivateHandler(repository, unitOfWork);

        var commandResult = MakeEventPrivateCommand.Create(Guid.NewGuid().ToString());
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("MakeEventPrivateHandler.HandleAsync", result.Error!.Code);
        Assert.Contains("not found", result.Error.Description);
    }
}
