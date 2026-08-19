using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class UpdateEventDescriptionEndpoint
    : ApiEndpoint
        .WithRequest<UpdateEventDescriptionRequest>
        .AndResults<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>
{
    private readonly ICommandDispatcher _commandDispatcher;

    public UpdateEventDescriptionEndpoint(ICommandDispatcher commandDispatcher) =>
        _commandDispatcher = commandDispatcher;

    [HttpPatch("events/update-event-description")]
    public override async Task<Results<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>> HandleAsync(
        UpdateEventDescriptionRequest request)
    {
        var commandResult = UpdateEventDescriptionCommand.Create(request.EventId, request.Description);

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

public sealed record UpdateEventDescriptionRequest(string EventId, string Description);
