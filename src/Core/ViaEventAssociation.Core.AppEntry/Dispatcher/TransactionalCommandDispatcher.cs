using ViaEventAssociation.Core.Domain.Common.UnitOfWork;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry.Dispatcher;

public class TransactionalCommandDispatcher : ICommandDispatcher
{
    private readonly ICommandDispatcher _next;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionalCommandDispatcher(ICommandDispatcher next, IUnitOfWork unitOfWork)
    {
        _next = next;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Result result = await _next.DispatchAsync(command);

        if (result.IsSuccess) await _unitOfWork.SaveChangesAsync();

        return result;
    }
}
