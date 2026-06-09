using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

public class EventAggregate : AggregateRoot<EventId>
{
    internal EventStatus Status { get; set; }
    internal MaxNumberOfGuests MaxNumberOfGuests { get; private set; }
    internal EventTitle Title { get; private set; }
    internal EventDescription Description { get; private set; }
    internal TimeInterval? TimeInterval { get; private set; }
    internal EventVisibility Visibility { get; set; }

    private EventAggregate(EventId id) : base(id)
    {
        Status = EventStatus.Draft;
        MaxNumberOfGuests = MaxNumberOfGuests.Create().Value;
        Title = EventTitle.Create().Value;
        Description = EventDescription.Create().Value;
        Visibility = EventVisibility.Private;
    }

    public static Result<EventAggregate> Create(EventId id) => new EventAggregate(id);

    public Result SetMaxNumberOfGuests(MaxNumberOfGuests maxNumberOfGuests)
    {
        if (Status == EventStatus.Cancelled)
        {
            return Result.Failure(Error.Validation("EventAggregate.Status", "A cancelled event cannot be modified."));
        }

        if (Status == EventStatus.Active && maxNumberOfGuests.Value < MaxNumberOfGuests.Value)
        {
            return Result.Failure(Error.Validation(
                "EventAggregate.MaxNumberOfGuests",
                "The maximum number of guests of an active event cannot be reduced."));
        }

        MaxNumberOfGuests = maxNumberOfGuests;

        return Result.Success();
    }

    public Result UpdateTitle(EventTitle title)
    {
        if (Status == EventStatus.Active)
            return Error.Validation("EventTitle.UpdateFailed", "An active event cannot be modified.");

        if (Status == EventStatus.Cancelled)
            return Error.Validation("EventTitle.UpdateFailed", "A cancelled event cannot be modified.");

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        Title = title;

        return Result.Success();
    }

    public Result UpdateDescription(EventDescription description)
    {
        if (Status == EventStatus.Active)
            return Error.Validation("EventDescription.UpdateFailed", "An active event cannot be modified.");

        if (Status == EventStatus.Cancelled)
            return Error.Validation("EventDescription.UpdateFailed", "A cancelled event cannot be modified.");

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        Description = description;

        return Result.Success();
    }

    public Result UpdateTimeInterval(DateTime startDateTime, DateTime endDateTime, DateTime currentTime)
    {
        if (Status == EventStatus.Active)
        {
            return Result.Failure(Error.Validation("EventAggregate.Status",
                "Times cannot be modified while the event is active."));
        }

        if (Status == EventStatus.Cancelled)
        {
            return Result.Failure(Error.Validation("EventAggregate.Status",
                "Times cannot be modified when the event is cancelled."));
        }

        var timeIntervalResult = TimeInterval.Create(startDateTime, endDateTime, currentTime);
        if (timeIntervalResult.HasErrors) return Result.Failure(timeIntervalResult.Error!);

        TimeInterval = timeIntervalResult.Value;

        if (Status == EventStatus.Ready)
        {
            Status = EventStatus.Draft;
        }

        return Result.Success();
    }

    public Result MarkAsPublic()
    {
        if (Status == EventStatus.Cancelled)
        {
            return Result.Failure(Error.Validation("EventAggregate.Status", "A cancelled event cannot be modified."));
        }

        Visibility = EventVisibility.Public;

        return Result.Success();
    }

    public Result MarkAsPrivate()
    {
        if (Status == EventStatus.Active)
        {
            return Result.Failure(Error.Validation("EventAggregate.Status", "An active event cannot be made private."));
        }

        if (Status == EventStatus.Cancelled)
        {
            return Result.Failure(Error.Validation("EventAggregate.Status", "A cancelled event cannot be modified."));
        }

        if (Visibility == EventVisibility.Public)
        {
            Visibility = EventVisibility.Private;
            Status = EventStatus.Draft;
        }

        return Result.Success();
    }
}
