using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class ViewSingleEventEndpoint
    : ApiEndpoint
        .WithRequest<ViewSingleEventRequest>
        .AndResults<Ok<ViewSingleEventResponse>, NotFound>
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IObjectMapper _objectMapper;

    public ViewSingleEventEndpoint(IQueryDispatcher queryDispatcher, IObjectMapper objectMapper)
    {
        _queryDispatcher = queryDispatcher;
        _objectMapper = objectMapper;
    }

    [HttpGet("events/{eventId}")]
    public override async Task<Results<Ok<ViewSingleEventResponse>, NotFound>> HandleAsync([FromRoute] ViewSingleEventRequest request)
    {
        var query = _objectMapper.Map<ViewSingleEventQuery.Query>(request);
        var answer = await _queryDispatcher.DispatchAsync<ViewSingleEventQuery.Query, ViewSingleEventQuery.Answer?>(query);

        if (answer is null) return TypedResults.NotFound();

        var response = _objectMapper.Map<ViewSingleEventResponse>(answer);
        return TypedResults.Ok(response);
    }
}

public sealed record ViewSingleEventRequest(string EventId);

public sealed record ViewSingleEventResponse(
    string Id,
    string Title,
    string Description,
    string? StartDateTime,
    string Visibility,
    int MaxNumberOfGuests
);

public class ViewSingleEventRequestToQueryConfig : IMappingConfig<ViewSingleEventRequest, ViewSingleEventQuery.Query>
{
    public ViewSingleEventQuery.Query Map(ViewSingleEventRequest input) =>
        new(input.EventId);
}

public class ViewSingleEventAnswerToResponseConfig : IMappingConfig<ViewSingleEventQuery.Answer, ViewSingleEventResponse>
{
    public ViewSingleEventResponse Map(ViewSingleEventQuery.Answer input) =>
        new(
            input.Id,
            input.Title,
            input.Description,
            input.StartDateTime,
            input.Visibility,
            input.MaxNumberOfGuests);
}
