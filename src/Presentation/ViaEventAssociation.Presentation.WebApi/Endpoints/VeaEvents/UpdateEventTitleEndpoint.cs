using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class UpdateEventTitleEndpoint
    : ApiEndpoint
        .WithRequest<UpdateEventTitleRequest>
        .AndResults<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>
{
    private readonly ICommandDispatcher _commandDispatcher;

    public UpdateEventTitleEndpoint(ICommandDispatcher commandDispatcher) =>
        _commandDispatcher = commandDispatcher;

    [HttpPatch("events/update-event-title")]
    public override async Task<Results<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>> HandleAsync(
        UpdateEventTitleRequest request)
    {
        var commandResult = UpdateEventTitleCommand.Create(request.EventId, request.Title);

        if (commandResult.HasErrors) return TypedResults.BadRequest(commandResult.Errors.ToList());

        var result = await _commandDispatcher.DispatchAsync(commandResult.Value);

        if (result.HasErrors)
        {
            return result.Error?.ErrorType == ErrorType.NotFound
                ? TypedResults.NotFound(result.Errors.ToList())
                : TypedResults.BadRequest(result.Errors.ToList());
        }

        return TypedResults.NoContent();
    }
}

public sealed record UpdateEventTitleRequest(string EventId, string Title);
