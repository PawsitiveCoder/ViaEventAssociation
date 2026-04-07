using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventDescription : ValueObject
{
    public const int MaxLength = 250;

    private readonly string _value;

    private EventDescription(string value) => _value = value;

    public static Result<EventDescription> Create(string? value)
    {
        var description = value ?? "";

        if (description.Length > MaxLength)
            return Error.Validation("EventDescription.Validation", $"Event description cannot exceed {MaxLength} characters.");

        return new EventDescription(description);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
