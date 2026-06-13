using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class ViewSingleEventEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_ReturnsOk()
    {
        var answer = new ViewSingleEventQuery.Answer(
            "event-id",
            "Board Games",
            "Game night",
            "2026-06-13T18:00:00",
            "Private",
            12);

        var endpoint = new ViewSingleEventEndpoint(
            new MockQueryDispatcher(answer),
            CreateMapper());

        var result = await endpoint.HandleAsync(new ViewSingleEventRequest("event-id"));

        var ok = Assert.IsType<Ok<ViewSingleEventResponse>>(result.Result);
        Assert.Equal("event-id", ok.Value.Id);
    }

    [Fact]
    public async Task HandleAsync_WhenEventDoesNotExist_ReturnsNotFound()
    {
        var endpoint = new ViewSingleEventEndpoint(
            new MockQueryDispatcher(null),
            CreateMapper());

        var result = await endpoint.HandleAsync(new ViewSingleEventRequest("missing-id"));

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new ViewSingleEventEndpoint(
            new MockQueryDispatcher(exception: new InvalidOperationException("boom")),
            CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => endpoint.HandleAsync(new ViewSingleEventRequest("event-id")));
    }

    private static IObjectMapper CreateMapper()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<ViewSingleEventRequest, ViewSingleEventQuery.Query>, ViewSingleEventRequestToQueryConfig>()
            .AddScoped<IMappingConfig<ViewSingleEventQuery.Answer, ViewSingleEventResponse>, ViewSingleEventAnswerToResponseConfig>()
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IObjectMapper>();
    }
}
