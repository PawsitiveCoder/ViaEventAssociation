using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.MakeEventPublic;

[TestSubject(typeof(MakeEventPublicHandler))]
public class MakeEventPublicHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_MarksEventAsPublic()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new MakeEventPublicHandler(repository);
        var eventAggregate = FakeEventAggregateFactory.Create();
        await repository.AddAsync(eventAggregate);

        var result = MakeEventPublicCommand.Create(eventAggregate.Id.Value.ToString());
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = Assert.Single(repository.Events);
        Assert.Equal(EventVisibility.Public, updatedEvent.Visibility);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new MakeEventPublicHandler(repository);

        var result = MakeEventPublicCommand.Create(Guid.NewGuid().ToString());
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.HasErrors);
        var error = Assert.Single(operationResult.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }
}
