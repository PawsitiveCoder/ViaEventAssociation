using JetBrains.Annotations;
using UnitTests.Fakes;
using UnitTests.Mocks;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateTitle;

[TestSubject(typeof(UpdateEventTitleHandler))]
public class UpdateEventTitleHandlerTests
{
    [Fact]
    public async Task UpdateEventTitle_Success()
    {
        var repository = new FakeEventAggregateRepository();
        var handler = new UpdateEventTitleHandler(repository);
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Draft);
        await repository.AddAsync(eventAggregate);
        string newTitle = "Test title";

        var result = UpdateEventTitleCommand.Create(eventAggregate.Id.Value.ToString(), newTitle);
        var command = result.Payload;

        Assert.NotNull(command);

        var operationResult = await handler.HandleAsync(command);

        Assert.True(operationResult.IsSuccess);
        var updatedEvent = repository.Events.First();
        Assert.Equal(newTitle, updatedEvent.Title.Value);
    }
}
