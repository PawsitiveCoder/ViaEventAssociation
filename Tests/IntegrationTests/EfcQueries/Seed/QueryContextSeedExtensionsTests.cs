namespace IntegrationTests.EfcQueries.Seed;

[TestSubject(typeof(QueryContextSeedExtensions))]
public class QueryContextSeedExtensionsTests
{
    [Fact]
    public async Task Seed_ShouldSeedTheEventAggregates()
    {
        await using QueryContext context = QueryContextHelpers.SetupContext().Seed();

        Assert.NotEmpty(context.EventAggregates);
        Assert.Equal(28, context.EventAggregates.Count());
    }
}
