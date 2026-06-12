namespace IntegrationTests.EfcQueries.QueryHandlers;

[TestSubject(typeof(BrowseUpcomingEventsQueryHandler))]
public class BrowseUpcomingEventsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOnlyUpcomingActiveEvents()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new BrowseUpcomingEventsQueryHandler(context, systemTime);
        var query = new BrowseUpcomingEventsQuery.Query();

        BrowseUpcomingEventsQuery.Answer answer = await handler.HandleAsync(query);

        Assert.Equal(7, answer.TotalItems);
        Assert.All(answer.Events, e =>
        {
            Assert.True(e.StartDateTime.CompareTo(systemTime.CurrentTime().ToString("yyyy-MM-ddTHH:mm:ss")) > 0);
        });
        Assert.Collection(answer.Events.Take(2),
            first =>
            {
                Assert.Equal("Garden Games", first.Title);
            },
            second =>
            {
                Assert.Equal("Yoga, introduction level", second.Title);
            });
    }

    [Fact]
    public async Task HandleAsync_ShouldApplySearchFilter()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 03, 01, 0, 0, 0, DateTimeKind.Utc));
        var handler = new BrowseUpcomingEventsQueryHandler(context, systemTime);
        var query = new BrowseUpcomingEventsQuery.Query("party");

        BrowseUpcomingEventsQuery.Answer answer = await handler.HandleAsync(query);

        Assert.Equal(2, answer.TotalItems);
        Assert.Equal(2, answer.Events.Count);
        Assert.Collection(answer.Events,
            first =>
            {
                Assert.Equal("Pizza Party", first.Title);
            },
            second =>
            {
                Assert.Equal("Lan Party", second.Title);
            });
    }

    [Fact]
    public async Task HandleAsync_ShouldAllowNavigationBetweenPages()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2010, 03, 01, 0, 0, 0, DateTimeKind.Utc));
        var handler = new BrowseUpcomingEventsQueryHandler(context, systemTime);

        var firstPage = await handler.HandleAsync(new BrowseUpcomingEventsQuery.Query());
        var secondPage = await handler.HandleAsync(new BrowseUpcomingEventsQuery.Query(2));

        Assert.Equal(1, firstPage.PageNumber);
        Assert.Equal(2, firstPage.TotalPages);
        Assert.Equal(17, firstPage.TotalItems);
        Assert.Equal(BrowseUpcomingEventsQuery.DefaultPageSize, firstPage.PageSize);
        Assert.Equal(BrowseUpcomingEventsQuery.DefaultPageSize, firstPage.Events.Count);

        Assert.Equal(2, secondPage.PageNumber);
        Assert.Equal(2, secondPage.TotalPages);
        Assert.Equal(17, secondPage.TotalItems);
        Assert.Equal(BrowseUpcomingEventsQuery.DefaultPageSize, secondPage.PageSize);
        Assert.Equal(5, secondPage.Events.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldCapDescriptionLength()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 03, 01, 0, 0, 0, DateTimeKind.Utc));
        var handler = new BrowseUpcomingEventsQueryHandler(context, systemTime);
        var query = new BrowseUpcomingEventsQuery.Query("Pizza Party");

        BrowseUpcomingEventsQuery.Answer answer = await handler.HandleAsync(query);

        var firstEvent = Assert.Single(answer.Events);
        Assert.Contains("...", firstEvent.Description);
        Assert.Equal(53, firstEvent.Description.Length);
    }
}
