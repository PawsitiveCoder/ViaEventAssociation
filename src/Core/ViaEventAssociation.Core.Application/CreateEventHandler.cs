using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Common.UnitOfWork;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class CreateEventHandler : ICommandHandler<CreateEventCommand>
{
    private readonly IEventAggregateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventHandler(IEventAggregateRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> HandleAsync(CreateEventCommand command)
    {
        var eventAggregate = EventAggregate.Create(command.Id);

        await _repository.AddAsync(eventAggregate.Value);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
