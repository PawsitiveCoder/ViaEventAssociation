using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class SetMaxNumberOfGuestsHandler : ICommandHandler<SetMaxNumberOfGuestsCommand>
{
    private readonly IEventAggregateRepository _repository;

    public SetMaxNumberOfGuestsHandler(IEventAggregateRepository repository) => _repository = repository;

    public async Task<Result> HandleAsync(SetMaxNumberOfGuestsCommand command)
    {
        var eventAggregate = await _repository.GetByIdAsync(command.EventId);

        return eventAggregate.Match(
            onSome: e => e.SetMaxNumberOfGuests(command.MaxNumberOfGuests),
            onNone: () => Error.NotFound("SetMaxNumberOfGuestsHandler.HandleAsync", $"Event with id {command.EventId.Value} not found")
        );
    }
}
