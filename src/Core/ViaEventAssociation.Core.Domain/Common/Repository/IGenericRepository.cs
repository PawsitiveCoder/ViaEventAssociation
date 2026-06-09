using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Common.Repository;

public interface IGenericRepository<T, in TId>
    where T : AggregateRoot<TId>
    where TId : ValueObject
{
    Task AddAsync(T aggregate);
    Task<T> GetByIdAsync(TId id);
    Task RemoveAsync(TId id);
}
