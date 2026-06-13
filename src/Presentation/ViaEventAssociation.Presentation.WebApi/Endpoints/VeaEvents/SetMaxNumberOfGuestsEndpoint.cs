using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class SetMaxNumberOfGuestsEndpoint
    : ApiEndpoint
        .WithRequest<SetMaxNumberOfGuestsRequest>
        .AndResults<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>
{
    private readonly ICommandDispatcher _commandDispatcher;

    public SetMaxNumberOfGuestsEndpoint(ICommandDispatcher commandDispatcher) =>
        _commandDispatcher = commandDispatcher;

    [HttpPatch("events/set-max-number-of-guests")]
    public override async Task<Results<NoContent, BadRequest<List<Error>>, NotFound<List<Error>>>> HandleAsync(
        SetMaxNumberOfGuestsRequest request)
    {
        var commandResult = SetMaxNumberOfGuestsCommand.Create(request.EventId, request.MaxNumberOfGuests);

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

public sealed record SetMaxNumberOfGuestsRequest(string EventId, int MaxNumberOfGuests);
