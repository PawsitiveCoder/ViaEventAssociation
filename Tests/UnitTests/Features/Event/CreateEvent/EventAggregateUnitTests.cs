using JetBrains.Annotations;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.CreateEvent;

[TestSubject(typeof(EventAggregate))]
public class EventAggregateUnitTests
{

    [Fact]
    public void CreateEvent_Success_EmptyEventCreated()
    {
        Result<EventAggregate> eventAggregate = EventAggregate.Create();

        Assert.True(eventAggregate.IsSuccess);
        Assert.NotNull(eventAggregate.Value);
        Assert.NotNull(eventAggregate.Value.Id);
        Assert.Equal(EventStatus.Draft, eventAggregate.Value.Status);
        Assert.Equal(MaxNumberOfGuests.DefaultValue, eventAggregate.Value.MaxNumberOfGuests.Value);
    }
}
