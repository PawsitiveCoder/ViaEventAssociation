using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application;

internal class UpdateEventTimeIntervalHandler : ICommandHandler<UpdateEventTimeIntervalCommand>
{
    private readonly IEventAggregateRepository _repository;

    public UpdateEventTimeIntervalHandler(IEventAggregateRepository repository) => _repository = repository;

    public async Task<Result> HandleAsync(UpdateEventTimeIntervalCommand command)
    {
        var eventAggregate = await _repository.GetByIdAsync(command.EventId);

        if (eventAggregate is null)
        {
            return Result.Failure(Error.NotFound("UpdateEventTimeIntervalHandler.HandleAsync", $"Event with id {command.EventId.Value} not found"));
        }

        var result = eventAggregate.UpdateTimeInterval(command.StartDateTime, command.EndDateTime, command.CurrentTime);

        return result;
    }
}
