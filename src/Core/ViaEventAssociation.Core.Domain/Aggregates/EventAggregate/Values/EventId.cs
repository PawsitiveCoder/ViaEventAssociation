using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventId : ValueObject
{
    private readonly Guid _value;

    private EventId(Guid value) => _value = value;

    public static Result<EventId> Create() => new EventId(Guid.NewGuid());

    public static Result<EventId> FromString(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            return Error.Validation("EventId.Validation", "EventId must be a valid GUID string.");

        return new EventId(guid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
