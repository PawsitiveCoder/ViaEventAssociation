using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public class DmContext : DbContext
{
    public DbSet<EventAggregate> EventAggregates => Set<EventAggregate>();

    public DmContext(DbContextOptions<DmContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DmContext).Assembly);
}
