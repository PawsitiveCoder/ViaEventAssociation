using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class BrowseUpcomingEventsEndpoint
    : ApiEndpoint
        .WithRequest<BrowseUpcomingEventsRequest>
        .AndResult<Ok<BrowseUpcomingEventsResponse>>
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IObjectMapper _objectMapper;

    public BrowseUpcomingEventsEndpoint(IQueryDispatcher queryDispatcher, IObjectMapper objectMapper)
    {
        _queryDispatcher = queryDispatcher;
        _objectMapper = objectMapper;
    }

    [HttpGet("events/browse-upcoming")]
    public override async Task<Ok<BrowseUpcomingEventsResponse>> HandleAsync([FromQuery] BrowseUpcomingEventsRequest request)
    {
        var query = _objectMapper.Map<BrowseUpcomingEventsQuery.Query>(request);
        var answer = await _queryDispatcher.DispatchAsync<BrowseUpcomingEventsQuery.Query, BrowseUpcomingEventsQuery.Answer>(query);
        var response = _objectMapper.Map<BrowseUpcomingEventsResponse>(answer);

        return TypedResults.Ok(response);
    }
}

public sealed record BrowseUpcomingEventsRequest(
    string? SearchText,
    int? PageNumber,
    int? PageSize
);

public sealed record BrowseUpcomingEventsResponse(
    IReadOnlyCollection<UpcomingEventListItemResponse> Events,
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages
);

public sealed record UpcomingEventListItemResponse(
    string Id,
    string Title,
    string Description,
    string StartDateTime,
    string Visibility,
    int MaxNumberOfGuests
);

public class BrowseUpcomingEventsRequestToQueryConfig : IMappingConfig<BrowseUpcomingEventsRequest, BrowseUpcomingEventsQuery.Query>
{
    public BrowseUpcomingEventsQuery.Query Map(BrowseUpcomingEventsRequest input) =>
        new(input.SearchText, input.PageNumber, input.PageSize);
}

public class BrowseUpcomingEventsAnswerToResponseConfig : IMappingConfig<BrowseUpcomingEventsQuery.Answer, BrowseUpcomingEventsResponse>
{
    public BrowseUpcomingEventsResponse Map(BrowseUpcomingEventsQuery.Answer input) =>
        new(
            input.Events.Select(e =>
                new UpcomingEventListItemResponse(
                    e.Id,
                    e.Title,
                    e.Description,
                    e.StartDateTime,
                    e.Visibility,
                    e.MaxNumberOfGuests)).ToList(),
            input.PageNumber,
            input.PageSize,
            input.TotalItems,
            input.TotalPages);
}
