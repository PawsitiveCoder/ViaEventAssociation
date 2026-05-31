using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventVisibility : ValueObject
{
    public static readonly EventVisibility Public = new("Public");
    public static readonly EventVisibility Private = new("Private");

    public string Value { get; }

    private EventVisibility(string value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
