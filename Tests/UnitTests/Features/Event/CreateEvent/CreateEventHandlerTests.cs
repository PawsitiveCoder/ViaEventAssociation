using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;

namespace UnitTests.Features.Event.CreateEvent;

[TestSubject(typeof(CreateEventHandler))]
public class CreateEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesEvent()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new CreateEventHandler(repository);

        var result = CreateEventCommand.Create();
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var createdEvent = Assert.Single(repository.Events);
        Assert.Equal(command.Id, createdEvent.Id);
    }
}
