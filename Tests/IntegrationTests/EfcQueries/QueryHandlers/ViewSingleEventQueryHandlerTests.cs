namespace IntegrationTests.EfcQueries.QueryHandlers;

[TestSubject(typeof(ViewSingleEventQueryHandler))]
public class ViewSingleEventQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAllEventFields_WhenEventExists()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var handler = new ViewSingleEventQueryHandler(context);
        const string EventId = "23a28a9a-2380-468d-9afc-c5cc1cda66f5";
        var query = new ViewSingleEventQuery.Query(EventId);

        ViewSingleEventQuery.Answer? answer = await handler.HandleAsync(query);

        Assert.NotNull(answer);
        Assert.Equal(EventId, answer.Id);
        Assert.Equal("Garden Games", answer.Title);
        Assert.StartsWith("Join us for a fun afternoon with all the classic garden games", answer.Description);
        Assert.Equal("2024-04-05T14:00:00", answer.StartDateTime);
        Assert.Equal(EventVisibility.Public.Value, answer.Visibility, ignoreCase: true);
        Assert.Equal(50, answer.MaxNumberOfGuests);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenEventDoesNotExist()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();
        var handler = new ViewSingleEventQueryHandler(context);
        const string EventId = "00000000-0000-0000-0000-000000000000";
        var query = new ViewSingleEventQuery.Query(EventId);

        ViewSingleEventQuery.Answer? answer = await handler.HandleAsync(query);

        Assert.Null(answer);
    }
}
