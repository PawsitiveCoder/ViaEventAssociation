using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

public class EventAggregate : AggregateRoot<EventId>
{
    public EventStatus Status { get; private set; }
    public MaxNumberOfGuests MaxNumberOfGuests { get; private set; }
    public EventTitle Title { get; private set; }
    public EventDescription Description { get; private set; }
    public TimeInterval? TimeInterval { get; private set; }

    private EventAggregate(EventId id) : base(id)
    {
        Status = EventStatus.Draft;
        MaxNumberOfGuests = MaxNumberOfGuests.Create().Value;
        Title = EventTitle.Create().Value;
        Description = EventDescription.Create().Value;
    }

    public static Result<EventAggregate> Create() => new EventAggregate(EventId.Create().Value);

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

    public Result UpdateTimeInterval(DateTime startDateTime, DateTime endDateTime)
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

        var timeIntervalResult = TimeInterval.Create(startDateTime, endDateTime);
        if (timeIntervalResult.HasErrors) return Result.Failure(timeIntervalResult.Error!);

        TimeInterval = timeIntervalResult.Value;

        if (Status == EventStatus.Ready)
        {
            Status = EventStatus.Draft;
        }

        return Result.Success();
    }

}