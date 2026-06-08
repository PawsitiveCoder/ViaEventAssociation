using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class UpdateEventTimeIntervalCommand
{
    internal EventId EventId { get; }
    internal DateTime StartDateTime { get; }
    internal DateTime EndDateTime { get; }
    internal DateTime CurrentTime { get; }

    private UpdateEventTimeIntervalCommand(EventId eventId, DateTime startDateTime, DateTime endDateTime, DateTime currentTime)
    {
        EventId = eventId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        CurrentTime = currentTime;
    }

    public static Result<UpdateEventTimeIntervalCommand> Create(string id, DateTime startDateTime, DateTime endDateTime, DateTime? currentTime = null)
    {
        var eventIdResult = EventId.FromString(id);
        var effectiveCurrentTime = currentTime ?? DateTime.Now;

        // Since TimeInterval needs all three parameters and performs validation on them,
        // we can pre-validate here using the domain logic to ensure the command only holds a valid interval.
        var timeIntervalResult = TimeInterval.Create(startDateTime, endDateTime, effectiveCurrentTime);

        return Result
            .CombineResultsInto<UpdateEventTimeIntervalCommand>(eventIdResult, timeIntervalResult)
            .WithPayloadIfSuccess(() => new UpdateEventTimeIntervalCommand(eventIdResult.Value, startDateTime, endDateTime, effectiveCurrentTime));
    }
}
