using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class MakeEventPrivateCommand
{
    internal EventId EventId { get; }

    private MakeEventPrivateCommand(EventId eventId) => EventId = eventId;

    public static Result<MakeEventPrivateCommand> Create(string id)
    {
        var eventIdResult = EventId.FromString(id);

        if (eventIdResult.HasErrors) return Result.Failure<MakeEventPrivateCommand>(eventIdResult.Error);

        return new MakeEventPrivateCommand(eventIdResult.Value);
    }
}
