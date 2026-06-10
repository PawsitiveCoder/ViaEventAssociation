using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Tools.Option;
using ViaEventAssociation.Infrastructure.EfcDmPersistence;

namespace UnitTests.Fakes;

public class FakeEventAggregateRepository : RepositoryBase<EventAggregate, EventId>, IEventAggregateRepository
{
    public List<EventAggregate> Events { get; private set; } = [];

    // TODO: Remove this and pass a mock context to the base constructor instead.
    public FakeEventAggregateRepository() : base(null)
    {
    }

    public FakeEventAggregateRepository(DmContext dmContext) : base(dmContext)
    {
    }

    public override async Task AddAsync(EventAggregate aggregate) =>
        Events.Add(aggregate);

    public override async Task<Option<EventAggregate>> GetByIdAsync(EventId id) =>
        Events.Find(e => e.Id == id);

    public override async Task<Result> RemoveAsync(EventId id)
    {
        var root = Events.Find(e => e.Id == id);

        if (root is null)
        {
            return Error.NotFound("EventAggregate.NotFound", $"Aggregate root with id {id} not found.");
        }

        Events.Remove(root);

        return Result.Success();
    }
}
