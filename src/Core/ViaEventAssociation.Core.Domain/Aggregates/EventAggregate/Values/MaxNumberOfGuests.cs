using System;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class MaxNumberOfGuests : ValueObject
{
    public static int DefaultValue { get; } = 5;
    public const int MinValue = 5;
    public const int MaxValue = 50;

    public int Value { get; }

    private MaxNumberOfGuests(int value) => Value = value;

    public static Result<MaxNumberOfGuests> Create() => Create(DefaultValue);

    public static Result<MaxNumberOfGuests> Create(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            return Error.Validation(
                "MaxNumberOfGuests.Validation",
                $"Maximum number of guests must be between {MinValue} and {MaxValue} inclusive.");
        }

        return new MaxNumberOfGuests(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
