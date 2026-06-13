namespace IntegrationTests.EfcQueries.QueryHandlers;

[TestSubject(typeof(EventsCalendarOverviewQueryHandler))]
public class EventsCalendarOverviewQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOnlyActiveEventsForRequestedMonth()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);
        var query = new EventsCalendarOverviewQuery.Query(2024, 4);

        EventsCalendarOverviewQuery.Answer answer = await handler.HandleAsync(query);

        Assert.Equal(2024, answer.Year);
        Assert.Equal(4, answer.Month);
        Assert.NotEmpty(answer.EventsByDay);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnActiveEventsForCurrentMonth_WhenYearMonthIsNull()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 03, 01, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);
        var query = new EventsCalendarOverviewQuery.Query();

        EventsCalendarOverviewQuery.Answer answer = await handler.HandleAsync(query);

        Assert.Equal(2024, answer.Year);
        Assert.Equal(3, answer.Month);
        Assert.NotEmpty(answer.EventsByDay);
    }

    [Fact]
    public async Task HandleAsync_ShouldGroupEventsByDay()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);
        var query = new EventsCalendarOverviewQuery.Query(2024, 4);

        EventsCalendarOverviewQuery.Answer answer = await handler.HandleAsync(query);

        Assert.True(answer.EventsByDay.ContainsKey(5), "Should have events on April 5");
        Assert.True(answer.EventsByDay.ContainsKey(12), "Should have events on April 12");
    }

    [Fact]
    public async Task HandleAsync_ShouldIncludeEventTitlesAndEventTimes()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);
        var query = new EventsCalendarOverviewQuery.Query(2024, 4);

        EventsCalendarOverviewQuery.Answer answer = await handler.HandleAsync(query);

        var day5Events = answer.EventsByDay[5];
        var gardenGamesEvent = day5Events.FirstOrDefault(e => e.Title == "Garden Games");

        Assert.NotNull(gardenGamesEvent);
        Assert.Equal("14:00", gardenGamesEvent.EventTime);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyDictionaryForMonthWithNoActiveEvents()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);
        var query = new EventsCalendarOverviewQuery.Query(2025, 1);

        EventsCalendarOverviewQuery.Answer answer = await handler.HandleAsync(query);

        Assert.Empty(answer.EventsByDay);
    }

    [Fact]
    public async Task HandleAsync_ShouldSortEventsByEventTimeWithinEachDay()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);
        var query = new EventsCalendarOverviewQuery.Query(2024, 3);

        EventsCalendarOverviewQuery.Answer answer = await handler.HandleAsync(query);

        if (answer.EventsByDay.TryGetValue(1, out var day1Events))
        {
            var eventTimes = day1Events.Select(e => e.EventTime).ToList();
            var sortedTimes = eventTimes.OrderBy(t => t).ToList();
            Assert.Equal(sortedTimes, eventTimes);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldAllowNavigationBetweenMonths()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var systemTime = new FakeSystemTime(new DateTime(2024, 04, 05, 0, 0, 0, DateTimeKind.Utc));
        var handler = new EventsCalendarOverviewQueryHandler(context, systemTime);

        var march = await handler.HandleAsync(new EventsCalendarOverviewQuery.Query(2024, 3));
        var april = await handler.HandleAsync(new EventsCalendarOverviewQuery.Query(2024, 4));

        Assert.NotEqual(march.EventsByDay.Count, april.EventsByDay.Count);
        Assert.Equal(2024, march.Year);
        Assert.Equal(3, march.Month);
        Assert.Equal(2024, april.Year);
        Assert.Equal(4, april.Month);
    }
}
