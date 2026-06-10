namespace IntegrationTests.DmContextConfiguration;

public static class DmContextHelpers
{
    public static DmContext SetupContext()
    {
        DbContextOptionsBuilder<DmContext> optionsBuilder = new();
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        optionsBuilder.UseSqlite(@"Data Source = " + testDbName);
        DmContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task SaveAndClearAsync<T>(T entity, DmContext context)
        where T : class
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
}
