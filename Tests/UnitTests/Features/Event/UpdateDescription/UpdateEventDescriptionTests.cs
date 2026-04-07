using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateDescription;

[TestSubject(typeof(EventAggregate))]
public class UpdateEventDescriptionTests
{
    private const string ValidDescription =
        "Nullam tempor lacus nisl, eget tempus quam maximus malesuada. Morbi faucibus sed neque vitae euismod. " +
        "Vestibulum non purus vel justo ornare vulputate. In a interdum enim. Maecenas sed sodales elit, sit amet " +
        "venenatis orci. Suspendisse potenti.";

    // S1
    [Fact]
    public void UpdateDescription_DraftEvent_ValidDescription_DescriptionUpdated()
    {
        var eventAggregate = EventAggregate.Create().Value;

        var result = eventAggregate.UpdateDescription(ValidDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventDescription.Create(ValidDescription).Value, eventAggregate.Description);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // S2
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void UpdateDescription_EmptyOrNullDescription_DescriptionSetToEmpty(string? description)
    {
        var eventAggregate = EventAggregate.Create().Value;

        var result = eventAggregate.UpdateDescription(description);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventDescription.Create("").Value, eventAggregate.Description);
    }

    // S3
    [Fact]
    public void UpdateDescription_ReadyEvent_ValidDescription_DescriptionUpdatedAndStatusSetToDraft()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Ready);

        var result = eventAggregate.UpdateDescription(ValidDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventDescription.Create(ValidDescription).Value, eventAggregate.Description);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // F1
    [Fact]
    public void UpdateDescription_TooLongDescription_Failure()
    {
        var eventAggregate = EventAggregate.Create().Value;
        var tooLong = new string('A', 251);

        var result = eventAggregate.UpdateDescription(tooLong);

        Assert.True(result.HasErrors);
        Assert.Contains("250", result.Error!.Description);
    }

    // F2
    [Fact]
    public void UpdateDescription_CancelledEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Cancelled);

        var result = eventAggregate.UpdateDescription(ValidDescription);

        Assert.True(result.HasErrors);
        Assert.Contains("cancelled", result.Error!.Description, StringComparison.OrdinalIgnoreCase);
    }

    // F3
    [Fact]
    public void UpdateDescription_ActiveEvent_Failure()
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Active);

        var result = eventAggregate.UpdateDescription(ValidDescription);

        Assert.True(result.HasErrors);
        Assert.Contains("active", result.Error!.Description, StringComparison.OrdinalIgnoreCase);
    }
}
