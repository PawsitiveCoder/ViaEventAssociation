using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

public class EventAggregate : AggregateRoot<EventId>
{
    public EventStatus Status { get; private set; }
    public MaxNumberOfGuests MaxNumberOfGuests { get; private set; }
    public EventTitle? Title { get; private set; }
    public EventDescription? Description { get; private set; }

    private EventAggregate(EventId id, EventStatus status, MaxNumberOfGuests maxNumberOfGuests) : base(id)
    {
        Status = status;
        MaxNumberOfGuests = maxNumberOfGuests;
    }

    public static Result<EventAggregate> Create()
    {
        var eventId = EventId.Create();
        var status = EventStatus.Draft;
        var maxNumberOfGuests = MaxNumberOfGuests.Create();

        return new EventAggregate(eventId.Value, status, maxNumberOfGuests.Value);
    }

    public Result UpdateTitle(string? title)
    {
        if (Status == EventStatus.Active)
            return Error.Validation("EventTitle.UpdateFailed", "An active event cannot be modified.");

        if (Status == EventStatus.Cancelled)
            return Error.Validation("EventTitle.UpdateFailed", "A cancelled event cannot be modified.");

        var titleResult = EventTitle.Create(title);
        if (titleResult.HasErrors)
            return titleResult.Error!;

        Title = titleResult.Value;

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        return Result.Success();
    }

    public Result UpdateDescription(string? description)
    {
        if (Status == EventStatus.Active)
            return Error.Validation("EventDescription.UpdateFailed", "An active event cannot be modified.");

        if (Status == EventStatus.Cancelled)
            return Error.Validation("EventDescription.UpdateFailed", "A cancelled event cannot be modified.");

        var descriptionResult = EventDescription.Create(description);
        if (descriptionResult.HasErrors)
            return descriptionResult.Error!;

        Description = descriptionResult.Value;

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        return Result.Success();
    }
}
