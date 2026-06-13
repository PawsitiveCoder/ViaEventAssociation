using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class EventsEditingOverviewEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenQuerySucceeds_ReturnsOk()
    {
        var answer = new EventsEditingOverviewQuery.Answer(
            [new EventsEditingOverviewQuery.EventListItem("draft-id", "Draft")],
            [new EventsEditingOverviewQuery.EventListItem("ready-id", "Ready")],
            [new EventsEditingOverviewQuery.EventListItem("cancelled-id", "Cancelled")]);

        var endpoint = new EventsEditingOverviewEndpoint(new MockQueryDispatcher(answer), CreateMapper());

        var result = await endpoint.HandleAsync();

        var ok = Assert.IsType<Ok<EventsEditingOverviewResponse>>(result);
        Assert.Single(ok.Value.DraftEvents);
        Assert.Single(ok.Value.ReadyEvents);
        Assert.Single(ok.Value.CancelledEvents);
    }

    [Fact]
    public async Task HandleAsync_WhenQueryReturnsNoItems_ReturnsOkWithEmptyLists()
    {
        var answer = new EventsEditingOverviewQuery.Answer([], [], []);

        var endpoint = new EventsEditingOverviewEndpoint(new MockQueryDispatcher(answer), CreateMapper());

        var result = await endpoint.HandleAsync();

        var ok = Assert.IsType<Ok<EventsEditingOverviewResponse>>(result);
        Assert.Empty(ok.Value.DraftEvents);
        Assert.Empty(ok.Value.ReadyEvents);
        Assert.Empty(ok.Value.CancelledEvents);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new EventsEditingOverviewEndpoint(
            new MockQueryDispatcher(exception: new InvalidOperationException("boom")),
            CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(endpoint.HandleAsync);
    }

    private static IObjectMapper CreateMapper()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<EventsEditingOverviewQuery.Answer, EventsEditingOverviewResponse>, EventsEditingOverviewAnswerToResponseConfig>()
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IObjectMapper>();
    }
}
