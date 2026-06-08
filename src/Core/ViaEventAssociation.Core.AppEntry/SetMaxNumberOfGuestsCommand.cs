using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class SetMaxNumberOfGuestsCommand
{
    internal EventId EventId { get; }
    internal MaxNumberOfGuests MaxNumberOfGuests { get; }

    private SetMaxNumberOfGuestsCommand(EventId eventId, MaxNumberOfGuests maxNumberOfGuests)
    {
        EventId = eventId;
        MaxNumberOfGuests = maxNumberOfGuests;
    }

    public static Result<SetMaxNumberOfGuestsCommand> Create(string id, int maxGuests)
    {
        var eventIdResult = EventId.FromString(id);
        var maxGuestsResult = MaxNumberOfGuests.Create(maxGuests);

        return Result
            .CombineResultsInto<SetMaxNumberOfGuestsCommand>(eventIdResult, maxGuestsResult)
            .WithPayloadIfSuccess(() => new SetMaxNumberOfGuestsCommand(eventIdResult.Value, maxGuestsResult.Value));
    }
}
