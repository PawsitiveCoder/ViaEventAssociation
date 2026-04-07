using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventId : ValueObject
{
    private readonly Guid _value;

    private EventId(Guid value) => _value = value;

    public static Result<EventId> Create() => new EventId(Guid.NewGuid());

    public static Result<EventId> FromGuid(Guid value) => new EventId(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
