using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Time;
using ViaEventAssociation.Infrastructure.EfcDmPersistence;
using ViaEventAssociation.Infrastructure.EfcQueries;

namespace IntegrationTests.WebAPI;

internal sealed class VeaWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"vea-int-{Guid.NewGuid():N}.sqlite");

    private string ConnectionString => $"Data Source={_dbPath}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DmContext>>();
            services.RemoveAll<DbContextOptions<QueryContext>>();
            services.RemoveAll<DmContext>();
            services.RemoveAll<QueryContext>();
            services.RemoveAll<ISystemTime>();

            services.AddDbContext<DmContext>(options => options.UseSqlite(ConnectionString));
            services.AddDbContext<QueryContext>(options => options.UseSqlite(ConnectionString));
            services.AddSingleton<ISystemTime>(new IntegrationTestsFakeTime());
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        DmContext dmContext = scope.ServiceProvider.GetRequiredService<DmContext>();

        await dmContext.Database.EnsureDeletedAsync();
        await dmContext.Database.EnsureCreatedAsync();
    }

    public async Task<string> SeedDraftEventAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        DmContext dmContext = scope.ServiceProvider.GetRequiredService<DmContext>();

        var eventId = EventId.Create().Value;
        var aggregate = EventAggregate.Create(eventId).Value;

        await dmContext.EventAggregates.AddAsync(aggregate);
        await dmContext.SaveChangesAsync();

        return eventId.Value.ToString();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        DeleteDbFile();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        DeleteDbFile();
    }

    private void DeleteDbFile()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }
}

internal sealed class IntegrationTestsFakeTime : ISystemTime
{
    public DateTime CurrentTime() => new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
}
