using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.SetMaxNumberOfGuests;

[TestSubject(typeof(EventAggregate))]
public class SetMaxNumberOfGuestsUnitTests
{
    // S1
    // S2
    [Theory]
    [InlineData(nameof(EventStatus.Draft), 5)]
    [InlineData(nameof(EventStatus.Draft), 10)]
    [InlineData(nameof(EventStatus.Draft), 25)]
    [InlineData(nameof(EventStatus.Draft), 50)]
    [InlineData(nameof(EventStatus.Ready), 5)]
    [InlineData(nameof(EventStatus.Ready), 10)]
    [InlineData(nameof(EventStatus.Ready), 25)]
    [InlineData(nameof(EventStatus.Ready), 50)]
    public void SetMaxNumberOfGuests_DraftOrReadyEvent_ValidValue_MaxGuestsUpdated(string statusName, int value)
    {
        var status = statusName switch
        {
            nameof(EventStatus.Draft) => EventStatus.Draft,
            nameof(EventStatus.Ready) => EventStatus.Ready,
            _ => throw new ArgumentException("Invalid status")
        };

        var eventAggregate = FakeEventAggregateFactory.Create(status);
        var maxNumberOfGuests = MaxNumberOfGuests.Create(value).Value;

        var result = eventAggregate.SetMaxNumberOfGuests(maxNumberOfGuests);

        Assert.True(result.IsSuccess);
        Assert.Equal(maxNumberOfGuests, eventAggregate.MaxNumberOfGuests);
    }

    // S3
    [Theory]
    [InlineData(5, 5)]
    [InlineData(15, 16)]
    [InlineData(50, 50)]
    public void SetMaxNumberOfGuests_ActiveEvent_IncreaseAllowed(int currentValue, int newValue)
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Active);
        var currentMaxValue = MaxNumberOfGuests.Create(currentValue).Value;
        eventAggregate.SetMaxNumberOfGuests(currentMaxValue);

        var newMaxValue = MaxNumberOfGuests.Create(newValue).Value;
        var result = eventAggregate.SetMaxNumberOfGuests(newMaxValue);

        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxValue, eventAggregate.MaxNumberOfGuests);
        Assert.Equal(EventStatus.Active, eventAggregate.Status);
    }

    // F1
    [Fact]
    public void SetMaxNumberOfGuests_ActiveEvent_DecreaseFailure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Active);
        var currentMaxValue = MaxNumberOfGuests.Create(25).Value;
        eventAggregate.SetMaxNumberOfGuests(currentMaxValue);

        var newMaxValue = MaxNumberOfGuests.Create(10).Value;
        var result = eventAggregate.SetMaxNumberOfGuests(newMaxValue);

        Assert.NotNull(result.Error);
        Assert.Equal(currentMaxValue, eventAggregate.MaxNumberOfGuests);
        Assert.Equal(EventStatus.Active, eventAggregate.Status);
        Assert.Contains("cannot be reduced", result.Error.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F2
    [Fact]
    public void SetMaxNumberOfGuests_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Cancelled);
        var maxNumberOfGuests = MaxNumberOfGuests.Create(25).Value;

        var result = eventAggregate.SetMaxNumberOfGuests(maxNumberOfGuests);

        Assert.NotNull(result.Error);
        Assert.Equal(5, eventAggregate.MaxNumberOfGuests.Value);
        Assert.Equal(EventStatus.Cancelled, eventAggregate.Status);
        Assert.Contains("cancelled", result.Error.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F3 Skipped, part of UC16-20

    // F4
    // F5
    [Theory]
    [InlineData(MaxNumberOfGuests.MinValue - 1)]
    [InlineData(MaxNumberOfGuests.MaxValue + 1)]
    public void SetMaxNumberOfGuests_GuestNumberOutOfRange_Failure(int invalidValue)
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Draft);
        var invalidMaxNumberOfGuests = MaxNumberOfGuests.Create(invalidValue);

        Assert.NotNull(invalidMaxNumberOfGuests.Error);
        Assert.Equal(MaxNumberOfGuests.MinValue, eventAggregate.MaxNumberOfGuests.Value);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
        Assert.Contains("between 5 and 50", invalidMaxNumberOfGuests.Error.Description, StringComparison.OrdinalIgnoreCase);
    }
}
