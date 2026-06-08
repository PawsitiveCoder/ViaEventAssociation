using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class UpdateEventDescriptionCommand
{
    internal EventId EventId { get; }
    internal EventDescription EventDescription { get; }

    private UpdateEventDescriptionCommand(EventId eventId, EventDescription eventDescription) =>
        (EventId, EventDescription) = (eventId, eventDescription);

    public static Result<UpdateEventDescriptionCommand> Create(string id, string description)
    {
        var eventIdResult = EventId.FromString(id);
        var eventDescriptionResult = EventDescription.Create(description);

        return Result
            .CombineResultsInto<UpdateEventDescriptionCommand>(eventIdResult, eventDescriptionResult)
            .WithPayloadIfSuccess(() => new UpdateEventDescriptionCommand(eventIdResult.Value, eventDescriptionResult.Value));
    }
}
