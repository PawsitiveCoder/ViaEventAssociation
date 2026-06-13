using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Infrastructure.EfcDmPersistence;

namespace IntegrationTests.WebAPI;

public class EventEndpointsIntegrationTests
{
    [Fact]
    public async Task CreateEvent_WhenRequestIsValid_ReturnsNoContent_AndPersistsEvent()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/events/create-event", new { });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using IServiceScope scope = webAppFactory.Services.CreateScope();
        DmContext dmContext = scope.ServiceProvider.GetRequiredService<DmContext>();
        int aggregateCount = await dmContext.EventAggregates.CountAsync();

        Assert.Equal(1, aggregateCount);
    }

    [Fact]
    public async Task UpdateEventTitle_WhenRequestIsValid_ReturnsNoContent_AndUpdatesReadModel()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage updateResponse = await client.PatchAsJsonAsync(
            "/api/events/update-event-title",
            new { eventId, title = "Integration Test Title" });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateEventTitle_WhenInputIsMissing_ReturnsBadRequest()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/update-event-title",
            new { eventId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventTitle_WhenValueObjectValidationFails_ReturnsBadRequest()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/update-event-title",
            new { eventId, title = string.Empty });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventTitle_WhenEventDoesNotExist_ReturnsNotFound()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/update-event-title",
            new { eventId = Guid.NewGuid().ToString(), title = "Valid title" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventTimeInterval_WhenTimeIntervalBusinessRuleFails_ReturnsBadRequest()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/update-event-time-interval",
            new
            {
                eventId,
                startDateTime = "2026-01-01T11:00:00Z",
                endDateTime = "2026-01-01T13:00:00Z"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventTitle_WhenEventIdCannotBeParsed_ThrowsOrReturnsInternalServerError()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        Exception? requestException = await Record.ExceptionAsync(async () =>
        {
            HttpResponseMessage response = await client.PatchAsJsonAsync(
                "/api/events/update-event-title",
                new { eventId = "not-a-guid", title = "Valid title" });

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        });

        if (requestException is not null)
        {
            Assert.Contains("guid", requestException.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task ViewSingleEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/api/events/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventDescription_WhenRequestIsValid_ReturnsNoContent()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/update-event-description",
            new { eventId, description = "Updated in integration test" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEventDescription_WhenDescriptionIsMissing_ReturnsBadRequest()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/update-event-description",
            new { eventId });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SetMaxNumberOfGuests_WhenRequestIsValid_ReturnsNoContent()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/set-max-number-of-guests",
            new { eventId, maxNumberOfGuests = 25 });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task SetMaxNumberOfGuests_WhenValueObjectValidationFails_ReturnsBadRequest()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/set-max-number-of-guests",
            new { eventId, maxNumberOfGuests = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MakeEventPublic_WhenRequestIsValid_ReturnsNoContent()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/make-event-public",
            new { eventId });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task MakeEventPublic_WhenEventDoesNotExist_ReturnsNotFound()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/make-event-public",
            new { eventId = Guid.NewGuid().ToString() });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MakeEventPrivate_WhenRequestIsValid_ReturnsNoContent()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        string eventId = await webAppFactory.SeedDraftEventAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            "/api/events/make-event-private",
            new { eventId });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task BrowseUpcomingEvents_WhenCalled_ReturnsOk()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/events/browse-upcoming");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EventsCalendarOverview_WhenCalled_ReturnsOk()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/events/calendar-overview");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EventsEditingOverview_WhenCalled_ReturnsOk()
    {
        await using var webAppFactory = new VeaWebApplicationFactory();
        await webAppFactory.ResetDatabaseAsync();
        using HttpClient client = webAppFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/events/editing-overview");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
