using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class MakeEventPublicHandler : ICommandHandler<MakeEventPublicCommand>
{
    private readonly IEventAggregateRepository _repository;

    public MakeEventPublicHandler(IEventAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(MakeEventPublicCommand command)
    {
        var eventAggregate = await _repository.GetByIdAsync(command.EventId.Value);

        if (eventAggregate is null)
        {
            return Result.Failure(Error.NotFound("MakeEventPublicHandler.HandleAsync", $"Event with id {command.EventId.Value} not found"));
        }

        var result = eventAggregate.MarkAsPublic();

        return result;
    }
}
