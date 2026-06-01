using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.MakeEventPrivate;

[TestSubject(typeof(EventAggregate))]
public class MakeEventPrivateUnitTests
{
    // S1
    [Theory]
    [InlineData(nameof(EventStatus.Draft))]
    [InlineData(nameof(EventStatus.Ready))]
    public void MarkAsPrivate_PrivateEvent_NoChanges(string statusName)
    {
        var status = statusName switch
        {
            nameof(EventStatus.Draft) => EventStatus.Draft,
            nameof(EventStatus.Ready) => EventStatus.Ready,
            _ => throw new ArgumentException("Invalid status")
        };
        var eventAggregate = FakeEventAggregateFactory.Create(status);

        var result = eventAggregate.MarkAsPrivate();

        Assert.True(result.IsSuccess);
        Assert.Equal(status, eventAggregate.Status);
        Assert.Equal(EventVisibility.Private, eventAggregate.Visibility);
    }

    // S2
    [Theory]
    [InlineData(nameof(EventStatus.Draft))]
    [InlineData(nameof(EventStatus.Ready))]
    public void MarkAsPrivate_PublicEvent_StatusSetToDraft(string statusName)
    {
        var status = statusName switch
        {
            nameof(EventStatus.Draft) => EventStatus.Draft,
            nameof(EventStatus.Ready) => EventStatus.Ready,
            _ => throw new ArgumentException("Invalid status")
        };
        var eventAggregate = FakeEventAggregateFactory.Create(status);

        eventAggregate.MarkAsPublic();
        var result = eventAggregate.MarkAsPrivate();

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
        Assert.Equal(EventVisibility.Private, eventAggregate.Visibility);
    }

    // F1
    [Fact]
    public void MarkAsPrivate_ActiveEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Active);

        eventAggregate.MarkAsPublic();
        var result = eventAggregate.MarkAsPrivate();

        Assert.NotNull(result.Error);
        Assert.Equal(EventStatus.Active, eventAggregate.Status);
        Assert.Equal(EventVisibility.Public, eventAggregate.Visibility);
        Assert.Contains("made private", result.Error.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F2
    [Fact]
    public void MarkAsPrivate_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Cancelled);

        var result = eventAggregate.MarkAsPrivate();

        Assert.NotNull(result.Error);
        Assert.Equal(EventStatus.Cancelled, eventAggregate.Status);
        Assert.Equal(EventVisibility.Private, eventAggregate.Visibility);
        Assert.Contains("cancelled", result.Error.Description, StringComparison.OrdinalIgnoreCase);
    }
}
