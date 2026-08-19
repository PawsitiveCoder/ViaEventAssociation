using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.QueryHandlers;

internal class EventsEditingOverviewQueryHandler : IQueryHandler<EventsEditingOverviewQuery.Query, EventsEditingOverviewQuery.Answer>
{
    private readonly QueryContext _context;

    public EventsEditingOverviewQueryHandler(QueryContext context) => _context = context;

    public async Task<EventsEditingOverviewQuery.Answer> HandleAsync(EventsEditingOverviewQuery.Query query)
    {
        var unpublishedEvents = await _context.EventAggregates
            .Where(e => e.Status.ToLower().Equals(EventStatus.Draft.Value.ToLower())
                        || e.Status.ToLower().Equals(EventStatus.Ready.Value.ToLower())
                        || e.Status.ToLower().Equals(EventStatus.Cancelled.Value.ToLower()))
            .OrderBy(e => e.StartDateTime)
            .Select(e => new
            {
                e.Id,
                e.Title,
                e.Status
            })
            .ToListAsync();

        var draftEvents = unpublishedEvents
            .Where(e => e.Status.Equals(EventStatus.Draft.Value, StringComparison.OrdinalIgnoreCase))
            .Select(e => new EventsEditingOverviewQuery.EventListItem(e.Id, e.Title))
            .ToList();

        var readyEvents = unpublishedEvents
            .Where(e => e.Status.Equals(EventStatus.Ready.Value, StringComparison.OrdinalIgnoreCase))
            .Select(e => new EventsEditingOverviewQuery.EventListItem(e.Id, e.Title))
            .ToList();

        var cancelledEvents = unpublishedEvents
            .Where(e => e.Status.Equals(EventStatus.Cancelled.Value, StringComparison.OrdinalIgnoreCase))
            .Select(e => new EventsEditingOverviewQuery.EventListItem(e.Id, e.Title))
            .ToList();

        return new EventsEditingOverviewQuery.Answer(draftEvents, readyEvents, cancelledEvents);
    }
}
