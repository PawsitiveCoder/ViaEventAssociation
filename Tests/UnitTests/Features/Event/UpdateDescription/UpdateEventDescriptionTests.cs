using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateDescription;

[TestSubject(typeof(EventAggregate))]
public class UpdateEventDescriptionTests
{
    private const string _validDescription =
        "Nullam tempor lacus nisl, eget tempus quam maximus malesuada. Morbi faucibus sed neque vitae euismod. " +
        "Vestibulum non purus vel justo ornare vulputate. In a interdum enim. Maecenas sed sodales elit, sit amet " +
        "venenatis orci. Suspendisse potenti.";

    // S1
    [Fact]
    public void UpdateDescription_DraftEvent_ValidDescription_DescriptionUpdated()
    {
        var eventAggregate = EventAggregate.Create().Value;
        var eventDescription = EventDescription.Create(_validDescription).Value;

        var result = eventAggregate.UpdateDescription(eventDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventDescription, eventAggregate.Description);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // S2
    [Theory]
    [InlineData("")]
    public void UpdateDescription_EmptyDescription_DescriptionSetToEmpty(string description)
    {
        var eventAggregate = EventAggregate.Create().Value;
        var eventDescription = EventDescription.Create(description).Value;

        var result = eventAggregate.UpdateDescription(eventDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventDescription, eventAggregate.Description);
    }

    // S3
    [Fact]
    public void UpdateDescription_ReadyEvent_ValidDescription_DescriptionUpdatedAndStatusSetToDraft()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Ready);
        var eventDescription = EventDescription.Create(_validDescription).Value;

        var result = eventAggregate.UpdateDescription(eventDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(eventDescription, eventAggregate.Description);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // F1
    [Fact]
    public void UpdateDescription_TooLongDescription_Failure()
    {
        var eventAggregate = EventAggregate.Create().Value;
        var tooLong = new string('A', 251);
        var eventDescription = EventDescription.Create(tooLong).Value;

        var result = eventAggregate.UpdateDescription(eventDescription);

        Assert.True(result.HasErrors);
        Assert.Contains("250", result.Error!.Description);
    }

    // F2
    [Fact]
    public void UpdateDescription_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Cancelled);
        var eventDescription = EventDescription.Create(_validDescription).Value;

        var result = eventAggregate.UpdateDescription(eventDescription);

        Assert.True(result.HasErrors);
        Assert.Equal(EventStatus.Cancelled, eventAggregate.Status);
        Assert.Contains("cancelled", result.Error!.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F3
    [Fact]
    public void UpdateDescription_ActiveEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Active);
        var eventDescription = EventDescription.Create(_validDescription).Value;

        var result = eventAggregate.UpdateDescription(eventDescription);

        Assert.True(result.HasErrors);
        Assert.Equal(EventStatus.Active, eventAggregate.Status);
        Assert.Contains("active", result.Error!.Description, StringComparison.OrdinalIgnoreCase);
    }
}
