using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class CreateEventHandler : ICommandHandler<CreateEventCommand>
{
    private readonly IEventAggregateRepository _repository;

    public CreateEventHandler(IEventAggregateRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result> HandleAsync(CreateEventCommand command)
    {
        var eventAggregate = EventAggregate.Create(command.Id);

        await _repository.AddAsync(eventAggregate.Value);

        return Result.Success();
    }
}
