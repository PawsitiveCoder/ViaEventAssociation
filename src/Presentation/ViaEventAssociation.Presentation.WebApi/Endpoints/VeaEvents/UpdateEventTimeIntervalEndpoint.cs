using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Domain.Common.Time;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class UpdateEventTimeIntervalEndpoint
    : ApiEndpoint
        .WithRequest<UpdateEventTimeIntervalRequest>
        .AndResults<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ISystemTime _systemTime;

    public UpdateEventTimeIntervalEndpoint(ICommandDispatcher commandDispatcher, ISystemTime systemTime)
    {
        _commandDispatcher = commandDispatcher;
        _systemTime = systemTime;
    }

    [HttpPatch("events/update-event-time-interval")]
    public override async Task<Results<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>> HandleAsync(
        UpdateEventTimeIntervalRequest request)
    {
        var commandResult = UpdateEventTimeIntervalCommand.Create(
            request.EventId,
            request.StartDateTime,
            request.EndDateTime,
            _systemTime.CurrentTime());

        if (commandResult.HasErrors) return TypedResults.BadRequest(commandResult.Errors);

        var result = await _commandDispatcher.DispatchAsync(commandResult.Value);

        if (result.HasErrors)
        {
            return result.Error?.ErrorType == ErrorType.NotFound
                ? TypedResults.NotFound(result.Errors)
                : TypedResults.BadRequest(result.Errors);
        }

        return TypedResults.NoContent();
    }
}

public sealed record UpdateEventTimeIntervalRequest(string EventId, DateTime StartDateTime, DateTime EndDateTime);
