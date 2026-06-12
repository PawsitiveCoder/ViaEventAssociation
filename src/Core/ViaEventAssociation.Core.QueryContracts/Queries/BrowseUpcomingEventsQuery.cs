using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public abstract class BrowseUpcomingEventsQuery
{
    public static readonly string DefaultSearchText = "";
    public static readonly int DefaultPageNumber = 1;
    public static readonly int DefaultPageSize = 12;

    public sealed record Query(
        string? SearchText,
        int? PageNumber,
        int? PageSize
    ) : IQuery<Answer>
    {
        public Query() : this(DefaultSearchText, DefaultPageNumber, DefaultPageSize) { }
        public Query(string SearchText) : this(SearchText, DefaultPageNumber, DefaultPageSize) { }
        public Query(int PageNumber) : this(DefaultSearchText, PageNumber, DefaultPageSize) { }
    }

    public sealed record Answer(
        IReadOnlyCollection<UpcomingEventListItem> Events,
        int PageNumber,
        int PageSize,
        int TotalItems,
        int TotalPages
    );

    public sealed record UpcomingEventListItem(
        string Id,
        string Title,
        string Description,
        string StartDateTime,
        string Visibility,
        int MaxNumberOfGuests
    );
}
