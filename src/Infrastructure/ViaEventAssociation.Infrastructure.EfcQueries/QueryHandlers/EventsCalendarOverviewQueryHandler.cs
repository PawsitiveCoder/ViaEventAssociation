using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Time;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.QueryHandlers;

internal class EventsCalendarOverviewQueryHandler : IQueryHandler<EventsCalendarOverviewQuery.Query, EventsCalendarOverviewQuery.Answer>
{
    private readonly QueryContext _context;
    private readonly ISystemTime _systemTime;

    public EventsCalendarOverviewQueryHandler(QueryContext context, ISystemTime systemTime)
    {
        _context = context;
        _systemTime = systemTime;
    }

    public async Task<EventsCalendarOverviewQuery.Answer> HandleAsync(EventsCalendarOverviewQuery.Query query)
    {
        var currentTime = _systemTime.CurrentTime();
        var year = query.Year ?? currentTime.Year;
        var month = query.Month ?? currentTime.Month;

        var events = await _context.EventAggregates
            .Where(e => e.Status.ToLower().Equals(EventStatus.Active.Value.ToLower()))
            .Where(e => e.StartDateTime != null && e.StartDateTime.StartsWith($"{year:D4}-{month:D2}-"))
            .ToListAsync();

        var result = events
            .Select(e =>
            {
                DateTime startDateTime = DateTime.Parse(
                    e.StartDateTime!,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);

                return new
                {
                    e.Id,
                    e.Title,
                    EventDay = startDateTime.Day,
                    EventTime = startDateTime.ToString("HH':'mm", CultureInfo.InvariantCulture)
                };
            })
            .GroupBy(e => e.EventDay)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyCollection<EventsCalendarOverviewQuery.EventOnDay>)group
                    .OrderBy(e => e.EventTime)
                    .Select(e => new EventsCalendarOverviewQuery.EventOnDay(e.Id, e.Title, e.EventTime))
                    .ToList()
            );

        return new EventsCalendarOverviewQuery.Answer(year, month, result);
    }
}
