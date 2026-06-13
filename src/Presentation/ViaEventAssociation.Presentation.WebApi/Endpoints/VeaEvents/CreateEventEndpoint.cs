using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class CreateEventEndpoint
    : ApiEndpoint
        .WithoutRequest
        .AndResults<NoContent, BadRequest<List<Error>>>
{
    private readonly ICommandDispatcher _commandDispatcher;

    public CreateEventEndpoint(ICommandDispatcher commandDispatcher) =>
        _commandDispatcher = commandDispatcher;

    [HttpPost("events/create-event")]
    public override async Task<Results<NoContent, BadRequest<List<Error>>>> HandleAsync()
    {
        var commandResult = CreateEventCommand.Create();

        if (commandResult.HasErrors) return TypedResults.BadRequest(commandResult.Errors);

        var result = await _commandDispatcher.DispatchAsync(commandResult.Value);

        if (result.HasErrors) return TypedResults.BadRequest(result.Errors);

        return TypedResults.NoContent();
    }
}

public record CreateEventRequest;
