using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class EventsEditingOverviewEndpoint
    : ApiEndpoint
        .WithoutRequest
        .AndResult<Ok<EventsEditingOverviewResponse>>
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IObjectMapper _objectMapper;

    public EventsEditingOverviewEndpoint(IQueryDispatcher queryDispatcher, IObjectMapper objectMapper)
    {
        _queryDispatcher = queryDispatcher;
        _objectMapper = objectMapper;
    }

    [HttpGet("events/editing-overview")]
    public override async Task<Ok<EventsEditingOverviewResponse>> HandleAsync()
    {
        var answer = await _queryDispatcher.DispatchAsync<EventsEditingOverviewQuery.Query, EventsEditingOverviewQuery.Answer>(
            new EventsEditingOverviewQuery.Query());

        var response = _objectMapper.Map<EventsEditingOverviewResponse>(answer);
        return TypedResults.Ok(response);
    }
}

public sealed record EventsEditingOverviewResponse(
    IReadOnlyCollection<EventListItemResponse> DraftEvents,
    IReadOnlyCollection<EventListItemResponse> ReadyEvents,
    IReadOnlyCollection<EventListItemResponse> CancelledEvents
);

public sealed record EventListItemResponse(
    string Id,
    string Title
);

internal class EventsEditingOverviewAnswerToResponseConfig : IMappingConfig<EventsEditingOverviewQuery.Answer, EventsEditingOverviewResponse>
{
    public EventsEditingOverviewResponse Map(EventsEditingOverviewQuery.Answer input) =>
        new(
            input.DraftEvents.Select(e => new EventListItemResponse(e.Id, e.Title)).ToList(),
            input.ReadyEvents.Select(e => new EventListItemResponse(e.Id, e.Title)).ToList(),
            input.CancelledEvents.Select(e => new EventListItemResponse(e.Id, e.Title)).ToList());
}
