using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class ViewSingleEventQuery
{
    public sealed record Query(
        string EventId
    ) : IQuery<Answer?>;

    public sealed record Answer(
        string Id,
        string Title,
        string Description,
        string? StartDateTime,
        string Visibility,
        int MaxNumberOfGuests
    );
}
