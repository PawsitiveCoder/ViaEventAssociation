using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class UpdateEventTitleHandler : ICommandHandler<UpdateEventTitleCommand>
{
    private readonly IEventAggregateRepository _repository;

    public UpdateEventTitleHandler(IEventAggregateRepository repository) => _repository = repository;

    public async Task<Result> HandleAsync(UpdateEventTitleCommand command)
    {
        var eventAggregate = await _repository.GetByIdAsync(command.EventId);

        return eventAggregate.Match(
            onSome: e => e.UpdateTitle(command.EventTitle),
            onNone: () => Error.NotFound("UpdateEventTitleHandler.HandleAsync", $"Event with id {command.EventId.Value} not found")
        );
    }
}
