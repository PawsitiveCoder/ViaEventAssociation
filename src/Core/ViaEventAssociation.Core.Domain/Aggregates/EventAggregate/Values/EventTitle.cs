using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class EventTitle : ValueObject
{
    public static string DefaultValue { get; } = "Working Title";
    public const int MinLength = 3;
    public const int MaxLength = 75;

    public string Value { get; }

    private EventTitle(string value) => Value = value;

    public static Result<EventTitle> Create() => Create(DefaultValue);

    public static Result<EventTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < MinLength || value.Length > MaxLength)
        {
            return Error.Validation("EventTitle.Validation", $"Event title must be between {MinLength} and {MaxLength} characters.");
        }

        return new EventTitle(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
