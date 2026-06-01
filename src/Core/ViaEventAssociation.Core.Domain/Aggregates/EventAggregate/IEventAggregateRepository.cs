namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

public interface IEventAggregateRepository
{
    Task AddAsync(EventAggregate eventAggregate);
    Task<EventAggregate?> GetByIdAsync(Guid id);
}
