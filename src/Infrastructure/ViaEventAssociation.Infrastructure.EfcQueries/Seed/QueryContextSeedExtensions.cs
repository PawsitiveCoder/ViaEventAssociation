
using ViaEventAssociation.Infrastructure.EfcQueries.Seed.Factories;

namespace ViaEventAssociation.Infrastructure.EfcQueries.Seed;

internal static class QueryContextSeedExtensions
{
    internal static QueryContext Seed(this QueryContext context)
    {
        var eventAggregates = EventAggregateSeedFactory.CreateEventAggregates();
        context.EventAggregates.AddRange(eventAggregates);
        context.SaveChanges();
        return context;
    }
}
