using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class BrowseUpcomingEventsEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenQuerySucceeds_ReturnsOk()
    {
        var answer = new BrowseUpcomingEventsQuery.Answer(
            [new BrowseUpcomingEventsQuery.UpcomingEventListItem("event-id", "Board Games", "Description", "2026-06-13T18:00:00", "Public", 20)],
            1,
            12,
            1,
            1);

        var endpoint = new BrowseUpcomingEventsEndpoint(new MockQueryDispatcher(answer), CreateMapper());

        var result = await endpoint.HandleAsync(new BrowseUpcomingEventsRequest("board", 1, 12));

        var ok = Assert.IsType<Ok<BrowseUpcomingEventsResponse>>(result);
        Assert.Single(ok.Value.Events);
    }

    [Fact]
    public async Task HandleAsync_WhenQueryReturnsEmpty_ReturnsOkWithNoItems()
    {
        var answer = new BrowseUpcomingEventsQuery.Answer([], 1, 12, 0, 0);

        var endpoint = new BrowseUpcomingEventsEndpoint(new MockQueryDispatcher(answer), CreateMapper());

        var result = await endpoint.HandleAsync(new BrowseUpcomingEventsRequest(null, null, null));

        var ok = Assert.IsType<Ok<BrowseUpcomingEventsResponse>>(result);
        Assert.Empty(ok.Value.Events);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new BrowseUpcomingEventsEndpoint(
            new MockQueryDispatcher(exception: new InvalidOperationException("boom")),
            CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => endpoint.HandleAsync(new BrowseUpcomingEventsRequest(null, 1, 12)));
    }

    private static IObjectMapper CreateMapper()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<BrowseUpcomingEventsRequest, BrowseUpcomingEventsQuery.Query>, BrowseUpcomingEventsRequestToQueryConfig>()
            .AddScoped<IMappingConfig<BrowseUpcomingEventsQuery.Answer, BrowseUpcomingEventsResponse>, BrowseUpcomingEventsAnswerToResponseConfig>()
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IObjectMapper>();
    }
}
