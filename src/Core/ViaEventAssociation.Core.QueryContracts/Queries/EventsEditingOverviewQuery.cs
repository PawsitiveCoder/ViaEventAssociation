using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class EventsEditingOverviewQuery
{
    public sealed record Query() : IQuery<Answer>;

    public sealed record Answer(
        IReadOnlyCollection<EventListItem> DraftEvents,
        IReadOnlyCollection<EventListItem> ReadyEvents,
        IReadOnlyCollection<EventListItem> CancelledEvents
    );

    public sealed record EventListItem(
        string Id,
        string Title
    );
}
