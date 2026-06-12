using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Time;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.QueryHandlers;

public class BrowseUpcomingEventsQueryHandler : IQueryHandler<BrowseUpcomingEventsQuery.Query, BrowseUpcomingEventsQuery.Answer>
{
    private readonly QueryContext _context;
    private readonly ISystemTime _systemTime;
    private const int _maxDescriptionLength = 50;

    public BrowseUpcomingEventsQueryHandler(QueryContext context, ISystemTime systemTime)
    {
        _context = context;
        _systemTime = systemTime;
    }

    public async Task<BrowseUpcomingEventsQuery.Answer> HandleAsync(BrowseUpcomingEventsQuery.Query query)
    {
        var currentTime = _systemTime.CurrentTime().ToString("yyyy-MM-ddTHH:mm:ss");
        var searchText = query.SearchText ?? BrowseUpcomingEventsQuery.DefaultSearchText;
        var pageNumber = query.PageNumber ?? BrowseUpcomingEventsQuery.DefaultPageNumber;
        var pageSize = query.PageSize ?? BrowseUpcomingEventsQuery.DefaultPageSize;

        var upcomingEvents = _context.EventAggregates
            .Where(e => e.Status.ToLower().Equals(EventStatus.Active.Value.ToLower()))
            .Where(e => e.Title.ToLower().Contains(searchText.ToLower()))
            .Where(e => e.StartDateTime != null && e.StartDateTime.CompareTo(currentTime) > 0)
            .OrderBy(e => e.StartDateTime);

        var pagedEvents = await upcomingEvents
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(e => new BrowseUpcomingEventsQuery.UpcomingEventListItem(
                e.Id,
                e.Title,
                e.Description.Substring(0, Math.Min(e.Description.Length, _maxDescriptionLength)) + (e.Description.Length > _maxDescriptionLength ? "..." : ""),
                e.StartDateTime!,
                e.Visibility,
                e.MaxNumberOfGuests))
            .ToListAsync();

        int totalItems = await upcomingEvents.CountAsync();
        int totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new BrowseUpcomingEventsQuery.Answer(
            pagedEvents,
            pageNumber,
            pageSize,
            totalItems,
            totalPages
        );
    }
}
