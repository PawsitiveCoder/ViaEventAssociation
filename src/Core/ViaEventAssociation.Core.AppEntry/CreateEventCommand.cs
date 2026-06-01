using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

public class CreateEventCommand
{
    internal EventId Id { get; }

    private CreateEventCommand(EventId id) => Id = id;

    public static Result<CreateEventCommand> Create()
    {
        var eventIdResult = EventId.Create();

        // TODO: Add helper method to avoid this boilerplate in all commands
        if (eventIdResult.Error is not null) return Result.Failure<CreateEventCommand>(eventIdResult.Error);

        return new CreateEventCommand(eventIdResult.Value);
    }
}
