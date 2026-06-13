using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;

public class EventsCalendarOverviewEndpoint
    : ApiEndpoint
        .WithRequest<EventsCalendarOverviewRequest>
        .AndResult<Ok<EventsCalendarOverviewResponse>>
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IObjectMapper _objectMapper;

    public EventsCalendarOverviewEndpoint(IQueryDispatcher queryDispatcher, IObjectMapper objectMapper)
    {
        _queryDispatcher = queryDispatcher;
        _objectMapper = objectMapper;
    }

    [HttpGet("events/calendar-overview")]
    public override async Task<Ok<EventsCalendarOverviewResponse>> HandleAsync([FromQuery] EventsCalendarOverviewRequest request)
    {
        var query = _objectMapper.Map<EventsCalendarOverviewQuery.Query>(request);
        var answer = await _queryDispatcher.DispatchAsync<EventsCalendarOverviewQuery.Query, EventsCalendarOverviewQuery.Answer>(query);
        var response = _objectMapper.Map<EventsCalendarOverviewResponse>(answer);

        return TypedResults.Ok(response);
    }
}

public sealed record EventsCalendarOverviewRequest(
    int? Year,
    int? Month
);

public sealed record EventsCalendarOverviewResponse(
    int Year,
    int Month,
    IReadOnlyDictionary<int, IReadOnlyCollection<EventOnDayResponse>> EventsByDay
);

public sealed record EventOnDayResponse(
    string Id,
    string Title,
    string EventTime
);

public class EventsCalendarOverviewRequestToQueryConfig : IMappingConfig<EventsCalendarOverviewRequest, EventsCalendarOverviewQuery.Query>
{
    public EventsCalendarOverviewQuery.Query Map(EventsCalendarOverviewRequest input) =>
        new(input.Year, input.Month);
}

public class EventsCalendarOverviewAnswerToResponseConfig : IMappingConfig<EventsCalendarOverviewQuery.Answer, EventsCalendarOverviewResponse>
{
    public EventsCalendarOverviewResponse Map(EventsCalendarOverviewQuery.Answer input) =>
        new(
            input.Year,
            input.Month,
            input.EventsByDay.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyCollection<EventOnDayResponse>)kvp.Value
                    .Select(e => new EventOnDayResponse(e.Id, e.Title, e.EventTime))
                    .ToList()));
}
