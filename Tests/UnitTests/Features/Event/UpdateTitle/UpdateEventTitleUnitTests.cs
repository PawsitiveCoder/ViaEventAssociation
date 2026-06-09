using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateTitle;

[TestSubject(typeof(EventAggregate))]
public class UpdateEventTitleTests
{
    // S1
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void UpdateTitle_DraftEvent_ValidTitle_TitleUpdated(string title)
    {
        var eventAggregate = FakeEventAggregateFactory.Create();
        var eventTitle = EventTitle.Create(title).Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventTitle, eventAggregate.Title);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // S2
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void UpdateTitle_ReadyEvent_ValidTitle_TitleUpdatedAndStatusSetToDraft(string title)
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Ready);
        var eventTitle = EventTitle.Create(title).Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventTitle, eventAggregate.Title);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // F1
    [Fact]
    public void UpdateTitle_EmptyTitle_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create();
        var emptyTitle = EventTitle.Create("");
        // TODO: Who should create the Object Value?
        // The aggregate or the calling method?
        var result = eventAggregate.UpdateTitle(emptyTitle.Value);

        Assert.NotNull(emptyTitle.Error);
        Assert.Contains("3 and 75", emptyTitle.Error.Description);
    }

    // F2
    [Theory]
    [InlineData("XY")]
    [InlineData("a")]
    public void UpdateTitle_TooShortTitle_Failure(string title)
    {
        var eventAggregate = FakeEventAggregateFactory.Create();
        var eventTitle = EventTitle.Create(title);

        var result = eventAggregate.UpdateTitle(eventTitle.Value);

        Assert.NotNull(eventTitle.Error);
        Assert.Contains("3 and 75", eventTitle.Error.Description);
    }

    // F3
    [Fact]
    public void UpdateTitle_TooLongTitle_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create();
        var longTitle = new string('A', 76);
        var eventTitle = EventTitle.Create(longTitle);

        var result = eventAggregate.UpdateTitle(eventTitle.Value);

        Assert.NotNull(eventTitle.Error);
        Assert.Contains("3 and 75", eventTitle.Error.Description);
    }

    // F4
    [Fact]
    public void UpdateTitle_NullTitle_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create();
        // TODO: Ask about this scenario, should it be able to receive null arg?
        // Also should the aggregate root receive Object Values as args?
        var eventTitle = EventTitle.Create(null);

        var result = eventAggregate.UpdateTitle(eventTitle.Value);

        Assert.NotNull(eventTitle.Error);
        Assert.Contains("3 and 75", eventTitle.Error.Description);
    }

    // F5
    [Fact]
    public void UpdateTitle_ActiveEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Active);
        var eventTitle = EventTitle.Create("Valid Title").Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.Equal(EventStatus.Active, eventAggregate.Status);
        Assert.NotNull(result.Error);
        Assert.Contains("active", result.Error.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F6
    [Fact]
    public void UpdateTitle_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.Create(EventStatus.Cancelled);
        var eventTitle = EventTitle.Create("Valid Title");

        var result = eventAggregate.UpdateTitle(eventTitle.Value);

        Assert.Equal(EventStatus.Cancelled, eventAggregate.Status);
        Assert.NotNull(result.Error);
        Assert.Contains("cancelled", result.Error.Description, StringComparison.OrdinalIgnoreCase);
    }
}
