using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class EventsCalendarOverviewQuery
{
    public sealed record Query(
        int? Year,
        int? Month
    ) : IQuery<Answer>
    {
        public Query() : this(null, null) { }
    }

    public sealed record Answer(
        int Year,
        int Month,
        IReadOnlyDictionary<int, IReadOnlyCollection<EventOnDay>> EventsByDay
    );

    public sealed record EventOnDay(
        string Id,
        string Title,
        string EventTime
    );
}
