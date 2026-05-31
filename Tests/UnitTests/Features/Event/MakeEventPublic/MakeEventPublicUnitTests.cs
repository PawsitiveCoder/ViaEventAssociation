using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.MakeEventPublic;

[TestSubject(typeof(EventAggregate))]
public class MakeEventPublicUnitTests
{
    // S1
    [Theory]
    [InlineData(nameof(EventStatus.Draft))]
    [InlineData(nameof(EventStatus.Ready))]
    [InlineData(nameof(EventStatus.Active))]
    public void MarkAsPublic_DraftReadyOrActiveEvent_VisibilityUpdated(string statusName)
    {
        var status = statusName switch
        {
            nameof(EventStatus.Draft) => EventStatus.Draft,
            nameof(EventStatus.Ready) => EventStatus.Ready,
            nameof(EventStatus.Active) => EventStatus.Active,
            _ => throw new ArgumentException("Invalid status")
        };
        var eventAggregate = FakeEventAggregateFactory.Create(status);

        var result = eventAggregate.MarkAsPublic();

        Assert.True(result.IsSuccess);
        Assert.Equal(status, eventAggregate.Status);
        Assert.Equal(EventVisibility.Public, eventAggregate.Visibility);
    }

    // F1
    [Fact]
    public void MarkAsPublic_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Cancelled);

        var result = eventAggregate.MarkAsPublic();

        Assert.True(result.HasErrors);
        Assert.Equal(EventVisibility.Private, eventAggregate.Visibility);
    }
}