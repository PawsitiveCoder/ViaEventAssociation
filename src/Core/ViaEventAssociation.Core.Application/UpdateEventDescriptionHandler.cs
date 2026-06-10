using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class UpdateEventDescriptionHandler : ICommandHandler<UpdateEventDescriptionCommand>
{
    private readonly IEventAggregateRepository _repository;

    public UpdateEventDescriptionHandler(IEventAggregateRepository repository) => _repository = repository;

    public async Task<Result> HandleAsync(UpdateEventDescriptionCommand command)
    {
        var eventAggregate = await _repository.GetByIdAsync(command.EventId);

        return eventAggregate.Match(
            onSome: e => e.UpdateDescription(command.EventDescription),
            onNone: () => Error.NotFound("UpdateEventDescriptionHandler.HandleAsync", $"Event with id {command.EventId.Value} not found")
        );
    }
}
