namespace IntegrationTests.EfcQueries.QueryHandlers;

[TestSubject(typeof(EventsEditingOverviewQueryHandler))]
public class EventsEditingOverviewQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOnlyUnpublishedEventsGroupedByStatus()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var handler = new EventsEditingOverviewQueryHandler(context);
        var query = new EventsEditingOverviewQuery.Query();

        EventsEditingOverviewQuery.Answer answer = await handler.HandleAsync(query);

        Assert.Equal(5, answer.DraftEvents.Count);
        Assert.Equal(4, answer.ReadyEvents.Count);
        Assert.Equal(2, answer.CancelledEvents.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotIncludeActiveEvents()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var handler = new EventsEditingOverviewQueryHandler(context);
        var query = new EventsEditingOverviewQuery.Query();

        EventsEditingOverviewQuery.Answer answer = await handler.HandleAsync(query);

        Assert.DoesNotContain(answer.DraftEvents, e => e.Title == "Friday Bar");
        Assert.DoesNotContain(answer.ReadyEvents, e => e.Title == "Friday Bar");
        Assert.DoesNotContain(answer.CancelledEvents, e => e.Title == "Friday Bar");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExpectedDraftTitles()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var handler = new EventsEditingOverviewQueryHandler(context);

        EventsEditingOverviewQuery.Answer answer = await handler.HandleAsync(new EventsEditingOverviewQuery.Query());

        var draftTitles = answer.DraftEvents.Select(e => e.Title).ToList();

        Assert.Contains("DnD introductions!", draftTitles);
        Assert.Contains("Whiskey Tasting", draftTitles);
        Assert.Contains("Card Stacking.", draftTitles);
        Assert.Contains("Soap Carving", draftTitles);
        Assert.Contains("Extreme Ironing", draftTitles);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExpectedReadyAndCancelledTitles()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var handler = new EventsEditingOverviewQueryHandler(context);

        EventsEditingOverviewQuery.Answer answer = await handler.HandleAsync(new EventsEditingOverviewQuery.Query());

        var readyTitles = answer.ReadyEvents.Select(e => e.Title).ToList();
        var cancelledTitles = answer.CancelledEvents.Select(e => e.Title).ToList();

        Assert.Contains("Chess and beer", readyTitles);
        Assert.Contains("Beer Tasting!", readyTitles);
        Assert.Contains("Art Exhibition", readyTitles);
        Assert.Contains("Juggling", readyTitles);

        Assert.Contains("Learn to knit!", cancelledTitles);
        Assert.Contains("Origami Introduction", cancelledTitles);
    }
}
