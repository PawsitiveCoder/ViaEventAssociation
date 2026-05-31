using JetBrains.Annotations;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.Event.UpdateTimeInterval;

[TestSubject(typeof(EventAggregate))]
public class UpdateTimeIntervalUnitTests
{
    // S1
    [Theory]
    [InlineData("2023-08-25T19:00:00", "2023-08-25T23:59:00")]
    [InlineData("2023-08-25T12:00:00", "2023-08-25T16:30:00")]
    [InlineData("2023-08-25T08:00:00", "2023-08-25T12:15:00")]
    [InlineData("2023-08-25T10:00:00", "2023-08-25T20:00:00")]
    [InlineData("2023-08-25T13:00:00", "2023-08-25T23:00:00")]
    public void UpdateTimeInterval_DraftStatus_SameDayValidTimes_TimeIntervalUpdated(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.IsSuccess);
        Assert.NotNull(eventAggregate.TimeInterval);
    }

    // S2
    [Theory]
    [InlineData("2023-08-25T19:00:00", "2023-08-26T01:00:00")]
    [InlineData("2023-08-25T12:00:00", "2023-08-25T16:30:00")]
    [InlineData("2023-08-25T08:00:00", "2023-08-25T12:15:00")]
    public void UpdateTimeInterval_DraftStatus_ValidTimes_TimeIntervalUpdated(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.IsSuccess);
        Assert.NotNull(eventAggregate.TimeInterval);
    }

    // S3
    [Theory]
    [InlineData("2026-08-25T12:00:00", "2026-08-25T16:30:00")]
    public void UpdateTimeInterval_ReadyStatus_ValidTimes_TimeIntervalUpdatedAndStatusSetToDraft(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Ready);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));


        Assert.True(result.IsSuccess);
        Assert.NotNull(eventAggregate.TimeInterval);
        Assert.Equal(EventStatus.Draft, eventAggregate.Status);
    }

    // S4
    [Theory]
    [InlineData("2026-08-25T12:00:00", "2026-08-25T16:30:00")]
    public void UpdateTimeInterval_FutureStart_TimeIntervalUpdated(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.IsSuccess);
    }

    // S5
    [Theory]
    [InlineData("2026-08-25T14:00:00", "2026-08-26T00:00:00")]
    public void UpdateTimeInterval_DurationAtMostTenHours_TimeIntervalUpdated(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.IsSuccess);
    }

    // F1
    [Theory]
    [InlineData("2023-08-26T19:00:00", "2023-08-25T01:00:00")]
    [InlineData("2023-08-26T19:00:00", "2023-08-25T23:59:00")]
    [InlineData("2023-08-27T12:00:00", "2023-08-25T16:30:00")]
    [InlineData("2023-08-01T08:00:00", "2023-07-31T12:15:00")]
    public void UpdateTimeInterval_StartDateAfterEndDate_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F2
    [Theory]
    [InlineData("2023-08-26T19:00:00", "2023-08-26T14:00:00")]
    [InlineData("2023-08-26T16:00:00", "2023-08-26T00:00:00")]
    [InlineData("2023-08-26T19:00:00", "2023-08-26T18:59:00")]
    [InlineData("2023-08-26T12:00:00", "2023-08-26T10:10:00")]
    [InlineData("2023-08-26T08:00:00", "2023-08-26T00:30:00")]
    public void UpdateTimeInterval_StartTimeAfterEndTime_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F3
    [Theory]
    [InlineData("2023-08-26T14:00:00", "2023-08-26T14:50:00")]
    [InlineData("2023-08-26T18:00:00", "2023-08-26T18:59:00")]
    [InlineData("2023-08-26T12:00:00", "2023-08-26T12:30:00")]
    [InlineData("2023-08-26T08:00:00", "2023-08-26T08:00:00")]
    public void UpdateTimeInterval_DurationTooShortSameDay_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F4
    [Theory]
    [InlineData("2023-08-25T23:30:00", "2023-08-26T00:15:00")]
    [InlineData("2023-08-30T23:01:00", "2023-08-31T00:00:00")]
    [InlineData("2023-08-30T23:59:00", "2023-08-31T00:01:00")]
    public void UpdateTimeInterval_DurationTooShortAcrossMidnight_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F5
    [Theory]
    [InlineData("2023-08-25T07:50:00", "2023-08-25T14:00:00")]
    [InlineData("2023-08-25T07:59:00", "2023-08-25T15:00:00")]
    [InlineData("2023-08-25T01:01:00", "2023-08-25T08:30:00")]
    [InlineData("2023-08-25T05:59:00", "2023-08-25T07:59:00")]
    [InlineData("2023-08-25T00:59:00", "2023-08-25T07:59:00")]
    public void UpdateTimeInterval_StartTimeBeforeEight_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F6
    [Theory]
    [InlineData("2023-08-24T23:50:00", "2023-08-25T01:01:00")]
    [InlineData("2023-08-24T22:00:00", "2023-08-25T07:59:00")]
    [InlineData("2023-08-30T23:00:00", "2023-08-31T02:30:00")]
    public void UpdateTimeInterval_OvernightEndAfterOne_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F7
    [Theory]
    [InlineData("2026-08-25T12:00:00", "2026-08-25T16:30:00")]
    public void UpdateTimeInterval_ActiveEvent_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Active);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F8
    [Theory]
    [InlineData("2026-08-25T12:00:00", "2026-08-25T16:30:00")]
    public void UpdateTimeInterval_CancelledEvent_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Cancelled);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F9
    [Theory]
    [InlineData("2023-08-30T08:00:00", "2023-08-30T18:01:00")]
    [InlineData("2023-08-30T14:59:00", "2023-08-31T01:00:00")]
    [InlineData("2023-08-30T14:00:00", "2023-08-31T00:01:00")]
    [InlineData("2023-08-30T14:00:00", "2023-08-31T18:30:00")]
    public void UpdateTimeInterval_DurationTooLong_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F10
    [Theory]
    [InlineData("2026-04-25T12:00:00", "2026-08-25T16:30:00")]
    public void UpdateTimeInterval_StartTimeInPast_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }

    // F11
    [Theory]
    [InlineData("2023-08-31T00:30:00", "2023-08-31T08:30:00")]
    [InlineData("2023-08-30T23:59:00", "2023-08-31T08:01:00")]
    [InlineData("2023-08-31T01:00:00", "2023-08-31T08:00:00")]
    public void UpdateTimeInterval_EventSpansInvalidNightWindow_Failure(string startIso, string endIso)
    {
        var eventAggregate = FakeEventAggregateFactory.WithStatus(EventStatus.Draft);

        var result = eventAggregate.UpdateTimeInterval(DateTime.Parse(startIso), DateTime.Parse(endIso));

        Assert.True(result.HasErrors);
    }
}
