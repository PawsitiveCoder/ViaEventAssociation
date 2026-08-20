using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventId : ValueObject
{
    public Guid Value { get; }

    private EventId(Guid value) => Value = value;

    public static Result<EventId> Create() => new EventId(Guid.NewGuid());

    public static Result<EventId> FromGuid(Guid value) => new EventId(value);

    public static Result<EventId> FromString(string value)
    {
        if (!Guid.TryParse(value, out Guid eventId))
        {
            return Error.Validation("EventId.Validation", "Event ID must be a valid GUID.");
        }

        return new EventId(eventId);
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
