using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;
using ViaEventAssociation.Core.Domain.Common.Repository;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;

public interface IEventAggregateRepository : IGenericRepository<EventAggregate, EventId>
{
}
