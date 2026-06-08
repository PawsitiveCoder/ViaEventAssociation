using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Common.UnitOfWork;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class MakeEventPrivateHandler : ICommandHandler<MakeEventPrivateCommand>
{
    private readonly IEventAggregateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MakeEventPrivateHandler(IEventAggregateRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

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
