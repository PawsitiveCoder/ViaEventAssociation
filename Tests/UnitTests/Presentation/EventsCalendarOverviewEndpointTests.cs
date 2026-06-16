using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class EventsCalendarOverviewEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenQuerySucceeds_ReturnsOk()
    {
        var eventsByDay = new Dictionary<int, IReadOnlyCollection<EventsCalendarOverviewQuery.EventOnDay>>
        {
            [15] =
            [
                new EventsCalendarOverviewQuery.EventOnDay("event-id", "Board Games", "18:00-20:00")
            ]
        };
        var answer = new EventsCalendarOverviewQuery.Answer(2026, 6, eventsByDay);

        var endpoint = new EventsCalendarOverviewEndpoint(new MockQueryDispatcher(answer), CreateMapper());

        var result = await endpoint.HandleAsync(new EventsCalendarOverviewRequest(2026, 6));

        var ok = Assert.IsType<Ok<EventsCalendarOverviewResponse>>(result);
        Assert.Equal(2026, ok.Value.Year);
        Assert.Equal(6, ok.Value.Month);
        Assert.Single(ok.Value.EventsByDay);
    }

    [Fact]
    public async Task HandleAsync_WhenNoEventsExist_ReturnsOkWithEmptyDays()
    {
        var answer = new EventsCalendarOverviewQuery.Answer(
            2026,
            6,
            new Dictionary<int, IReadOnlyCollection<EventsCalendarOverviewQuery.EventOnDay>>());

        var endpoint = new EventsCalendarOverviewEndpoint(new MockQueryDispatcher(answer), CreateMapper());

        var result = await endpoint.HandleAsync(new EventsCalendarOverviewRequest(null, null));

        var ok = Assert.IsType<Ok<EventsCalendarOverviewResponse>>(result);
        Assert.Empty(ok.Value.EventsByDay);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new EventsCalendarOverviewEndpoint(
            new MockQueryDispatcher(exception: new InvalidOperationException("boom")),
            CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            endpoint.HandleAsync(new EventsCalendarOverviewRequest(2026, 6)));
    }

    private static IObjectMapper CreateMapper()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<EventsCalendarOverviewRequest, EventsCalendarOverviewQuery.Query>, EventsCalendarOverviewRequestToQueryConfig>()
            .AddScoped<IMappingConfig<EventsCalendarOverviewQuery.Answer, EventsCalendarOverviewResponse>, EventsCalendarOverviewAnswerToResponseConfig>()
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IObjectMapper>();
    }
}
