namespace IntegrationTests.EfcDmPersistence;

public static class DmContextHelpers
{
    public static DmContext SetupContext()
    {
        DbContextOptionsBuilder<DmContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source=TestDmContext-{Guid.NewGuid()}.sqlite");
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
