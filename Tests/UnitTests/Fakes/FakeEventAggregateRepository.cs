using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

namespace UnitTests.Fakes;

public class FakeEventAggregateRepository : IEventAggregateRepository
{
    public List<EventAggregate> Events { get; private set; } = [];

    public Task AddAsync(EventAggregate eventAggregate)
    {
        Events.Add(eventAggregate);
        return Task.CompletedTask;
    }

    public Task<EventAggregate?> GetByIdAsync(Guid id)
    {
        var eventAggregate = Events.Find(e => e.Id.Value == id);
        return Task.FromResult(eventAggregate);
    }
}
