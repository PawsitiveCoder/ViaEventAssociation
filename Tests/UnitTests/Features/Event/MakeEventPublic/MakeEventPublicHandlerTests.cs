using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.MakeEventPublic;

[TestSubject(typeof(MakeEventPublicHandler))]
public class MakeEventPublicHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_MarksAsPublic()
    {
        var repository = new FakeEventAggregateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new MakeEventPublicHandler(repository, unitOfWork);

        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var commandResult = MakeEventPublicCommand.Create(eventAggregate.Id.Value.ToString());
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        var updatedEvent = await repository.GetByIdAsync(eventAggregate.Id.Value);
        Assert.Equal(EventVisibility.Public, updatedEvent!.Visibility);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new MakeEventPublicHandler(repository, unitOfWork);

        var commandResult = MakeEventPublicCommand.Create(Guid.NewGuid().ToString());
        var command = commandResult.Payload!;

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("MakeEventPublicHandler.HandleAsync", result.Error!.Code);
        Assert.Contains("not found", result.Error.Description);
    }
}
