namespace IntegrationTests.EfcQueries;

public static class QueryContextHelpers
{
    public static QueryContext SetupContext()
    {
        DbContextOptionsBuilder<QueryContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source=TestQueryContext-{Guid.NewGuid()}.sqlite");
        QueryContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }
}
