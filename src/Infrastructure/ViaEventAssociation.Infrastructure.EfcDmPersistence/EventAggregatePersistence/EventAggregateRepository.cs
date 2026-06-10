using System;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.EventAggregatePersistence;

public class EventAggregateRepository : RepositoryBase<EventAggregate, EventId>, IEventAggregateRepository
{
    public EventAggregateRepository(DmContext dmContext) : base(dmContext)
    {
    }
}
