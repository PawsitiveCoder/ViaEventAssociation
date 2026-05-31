using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Fakes;

public static class FakeEventAggregateFactory
{
    public static EventAggregate Create() => Create(EventStatus.Draft);

    public static EventAggregate Create(EventStatus status)
    {
        var aggregate = EventAggregate.Create().Value;
        typeof(EventAggregate)
            .GetProperty(nameof(EventAggregate.Status))!
            .SetValue(aggregate, status);
        return aggregate;
    }
}
