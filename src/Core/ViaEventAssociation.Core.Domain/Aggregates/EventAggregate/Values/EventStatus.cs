using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventStatus : ValueObject
{
    public static readonly EventStatus Draft = new("Draft");
    public static readonly EventStatus Ready = new("Ready");
    public static readonly EventStatus Active = new("Active");
    public static readonly EventStatus Cancelled = new("Cancelled");

    public string Value { get; }

    private EventStatus(string value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
