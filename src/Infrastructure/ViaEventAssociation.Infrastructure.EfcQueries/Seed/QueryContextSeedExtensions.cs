
using ViaEventAssociation.Infrastructure.EfcQueries.Seed.Factories;

namespace ViaEventAssociation.Infrastructure.EfcQueries.Seed;

public static class QueryContextSeedExtensions
{
    public static QueryContext Seed(this QueryContext context)
    {
        var eventAggregates = EventAggregateSeedFactory.CreateEventAggregates();
        context.EventAggregates.AddRange(eventAggregates);
        context.SaveChanges();
        return context;
    }
}
