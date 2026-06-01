using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class UpdateEventTitleCommand
{
    internal EventId EventId { get; }
    internal EventTitle EventTitle { get; }

    private UpdateEventTitleCommand(EventId eventId, EventTitle eventTitle) =>
        (EventId, EventTitle) = (eventId, eventTitle);

    public static Result<UpdateEventTitleCommand> Create(string id, string title)
    {
        var eventIdResult = EventId.FromString(id);
        var eventTitleResult = EventTitle.Create(title);

        return Result
            .CombineResultsInto<UpdateEventTitleCommand>(eventIdResult, eventTitleResult)
            .WithPayloadIfSuccess(() => new UpdateEventTitleCommand(eventIdResult.Value, eventTitleResult.Value));
    }
}
