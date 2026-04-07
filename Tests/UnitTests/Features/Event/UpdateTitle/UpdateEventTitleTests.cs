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
        var eventAggregate = EventAggregate.Create().Value;
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
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Ready);
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
        var eventAggregate = EventAggregate.Create().Value;
        var emptyTitle = EventTitle.Create("").Value;

        var result = eventAggregate.UpdateTitle(emptyTitle);

        Assert.True(result.HasErrors);
        Assert.Contains("3 and 75", result.Error!.Description);
    }

    // F2
    [Theory]
    [InlineData("XY")]
    [InlineData("a")]
    public void UpdateTitle_TooShortTitle_Failure(string title)
    {
        var eventAggregate = EventAggregate.Create().Value;
        var eventTitle = EventTitle.Create(title).Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.HasErrors);
        Assert.Contains("3 and 75", result.Error!.Description);
    }

    // F3
    [Fact]
    public void UpdateTitle_TooLongTitle_Failure()
    {
        var eventAggregate = EventAggregate.Create().Value;
        var longTitle = new string('A', 76);
        var eventTitle = EventTitle.Create(longTitle).Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.HasErrors);
        Assert.Contains("3 and 75", result.Error!.Description);
    }

    // F4
    [Fact]
    public void UpdateTitle_NullTitle_Failure()
    {
        var eventAggregate = EventAggregate.Create().Value;
        // TODO: ask about this scenario
        var eventTitle = EventTitle.Create(null).Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.HasErrors);
        Assert.Contains("3 and 75", result.Error!.Description);
    }

    // F5
    [Fact]
    public void UpdateTitle_ActiveEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Active);
        var eventTitle = EventTitle.Create("Valid Title").Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.HasErrors);
        Assert.Equal(EventStatus.Active, eventAggregate.Status);
        Assert.Contains("active", result.Error!.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F6
    [Fact]
    public void UpdateTitle_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Cancelled);
        var eventTitle = EventTitle.Create("Valid Title").Value;

        var result = eventAggregate.UpdateTitle(eventTitle);

        Assert.True(result.HasErrors);
        Assert.Equal(EventStatus.Cancelled, eventAggregate.Status);
        Assert.Contains("cancelled", result.Error!.Description, StringComparison.OrdinalIgnoreCase);
    }
}
