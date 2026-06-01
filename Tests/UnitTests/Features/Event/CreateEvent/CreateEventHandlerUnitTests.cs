using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.CreateEvent;

[TestSubject(typeof(CreateEventHandler))]
public class CreateEventHandlerUnitTests
{
    [Fact]
    public async Task GivenNothing_WhenCreatingEvent_ThenEventIsCreatedWithIdAndDefaultValues()
    {
        var repository = new FakeEventAggregateRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateEventHandler(repository, unitOfWork);

        var result = CreateEventCommand.Create();
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        Assert.Single(repository.Events);
        var createdEvent = repository.Events.First();
        Assert.Equal(command.Id, createdEvent.Id);
    }
}
