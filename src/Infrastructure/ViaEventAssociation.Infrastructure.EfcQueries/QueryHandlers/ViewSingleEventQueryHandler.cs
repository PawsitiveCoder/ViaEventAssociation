using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace ViaEventAssociation.Infrastructure.EfcQueries.QueryHandlers;

public class ViewSingleEventQueryHandler : IQueryHandler<ViewSingleEventQuery.Query, ViewSingleEventQuery.Answer?>
{
    private readonly QueryContext _context;

    public ViewSingleEventQueryHandler(QueryContext context) => _context = context;

    public async Task<ViewSingleEventQuery.Answer?> HandleAsync(ViewSingleEventQuery.Query query)
    {
        var eventAggregate = await _context.EventAggregates
            .FirstOrDefaultAsync(e => e.Id == query.EventId);

        if (eventAggregate == null)
        {
            return null;
        }

        return new ViewSingleEventQuery.Answer(
            eventAggregate.Id,
            eventAggregate.Title,
            eventAggregate.Description,
            eventAggregate.StartDateTime,
            eventAggregate.Visibility,
            eventAggregate.MaxNumberOfGuests
        );
    }
}
