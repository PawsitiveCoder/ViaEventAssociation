using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class MakeEventPrivateHandler : ICommandHandler<MakeEventPrivateCommand>
{
    private readonly IEventAggregateRepository _repository;

    public MakeEventPrivateHandler(IEventAggregateRepository repository) => _repository = repository;

    public async Task<Result> HandleAsync(MakeEventPrivateCommand command)
    {
        var eventAggregate = await _repository.GetByIdAsync(command.EventId.Value);

        if (eventAggregate is null)
        {
            return Result.Failure(Error.NotFound("MakeEventPrivateHandler.HandleAsync", $"Event with id {command.EventId.Value} not found"));
        }

        var result = eventAggregate.MarkAsPrivate();

        return result;
    }
}
