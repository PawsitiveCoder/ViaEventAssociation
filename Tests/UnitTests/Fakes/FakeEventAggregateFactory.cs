using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Fakes;

public static class FakeEventAggregateFactory
{
    public static EventAggregate Create() => Create(EventStatus.Draft);

    public static EventAggregate Create(EventStatus status)
    {
        var eventId = EventId.Create().Value;
        var aggregate = EventAggregate.Create(eventId).Value;
        aggregate.Status = status;
        return aggregate;
    }

    public static EventAggregate Create(EventVisibility visibility)
    {
        var eventAggregate = Create();
        eventAggregate.Visibility = visibility;
        return eventAggregate;
    }
}
