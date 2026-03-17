using System;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

public class MaxNumberOfGuests : ValueObject
{
    public static int DefaultValue { get; } = 5;
    public int Value { get; }

    private MaxNumberOfGuests(int value) => Value = value;

    public static Result<MaxNumberOfGuests> Create() => Create(DefaultValue);

    public static Result<MaxNumberOfGuests> Create(int value)
    {
        return new MaxNumberOfGuests(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
