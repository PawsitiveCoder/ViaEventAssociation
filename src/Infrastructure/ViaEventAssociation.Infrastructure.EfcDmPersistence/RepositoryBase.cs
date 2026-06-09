using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Domain.Common.Repository;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public abstract class RepositoryBase<T, TId> : IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : ValueObject
{
    private readonly DmContext _dmContext;

    public RepositoryBase(DmContext dmContext) => _dmContext = dmContext;

    public virtual async Task AddAsync(T aggregate) =>
        await _dmContext.Set<T>().AddAsync(aggregate);

    public virtual async Task<T> GetByIdAsync(TId id)
    {
        var root = await _dmContext.Set<T>().FindAsync(id);

        // TODO: Consider returning a Result<T> instead of throwing an exception.
        if (root is null)
            throw new InvalidOperationException($"Aggregate with id {id} not found.");

        return root;
    }

    public virtual async Task RemoveAsync(TId id)
    {
        var root = await _dmContext.Set<T>().FindAsync(id);

        // TODO: Consider returning a Result instead of throwing an exception.
        if (root is null)
            throw new InvalidOperationException($"Aggregate with id {id} not found.");

        _dmContext.Set<T>().Remove(root);
    }
}
