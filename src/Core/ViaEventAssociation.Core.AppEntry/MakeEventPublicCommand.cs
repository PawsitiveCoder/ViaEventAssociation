using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class MakeEventPublicCommand
{
    internal EventId EventId { get; }

    private MakeEventPublicCommand(EventId eventId) => EventId = eventId;

    public static Result<MakeEventPublicCommand> Create(string id)
    {
        var eventIdResult = EventId.FromString(id);

        return Result
            .CombineResultsInto<MakeEventPublicCommand>(eventIdResult)
            .WithPayloadIfSuccess(() => new MakeEventPublicCommand(eventIdResult.Value));
    }
}
