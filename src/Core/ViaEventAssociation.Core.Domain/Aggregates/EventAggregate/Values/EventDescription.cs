using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventDescription : ValueObject
{
    public static string DefaultValue { get; } = "";
    public const int MaxLength = 250;

    public string Value { get; }

    private EventDescription(string value) => Value = value;

    public static Result<EventDescription> Create() => Create(DefaultValue);

    public static Result<EventDescription> Create(string value)
    {
        if (value.Length > MaxLength)
            return Error.Validation("EventDescription.Validation", $"Event description cannot exceed {MaxLength} characters.");

        return new EventDescription(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
