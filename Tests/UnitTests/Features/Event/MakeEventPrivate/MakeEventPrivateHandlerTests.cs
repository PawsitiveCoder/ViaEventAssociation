using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.MakeEventPrivate;

[TestSubject(typeof(MakeEventPrivateHandler))]
public class MakeEventPrivateHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_MarksEventAsPrivate()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new MakeEventPrivateHandler(repository);
        var eventAggregate = FakeEventAggregateFactory.Create(EventVisibility.Public);
        await repository.AddAsync(eventAggregate);

        var result = MakeEventPrivateCommand.Create(eventAggregate.Id.Value.ToString());
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = Assert.Single(repository.Events);
        Assert.Equal(EventVisibility.Private, updatedEvent.Visibility);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFailure()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new MakeEventPrivateHandler(repository);

        var result = MakeEventPrivateCommand.Create(Guid.NewGuid().ToString());
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.HasErrors);
        var error = Assert.Single(operationResult.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }
}
