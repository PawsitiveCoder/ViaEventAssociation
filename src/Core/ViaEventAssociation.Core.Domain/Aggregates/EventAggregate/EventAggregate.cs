using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

public class EventAggregate : AggregateRoot<EventId>
{
    public EventStatus Status { get; private set; }
    public MaxNumberOfGuests MaxNumberOfGuests { get; private set; }

    private EventAggregate(EventId id, EventStatus status, MaxNumberOfGuests maxNumberOfGuests) : base(id)
    {
        Status = status;
        MaxNumberOfGuests = maxNumberOfGuests;
    }

    public static Result<EventAggregate> Create()
    {
        var eventId = EventId.Create();
        var status = EventStatus.Draft;
        var maxNumberOfGuests = MaxNumberOfGuests.Create();

        return new EventAggregate(eventId.Value, status, maxNumberOfGuests.Value);
    }
}
