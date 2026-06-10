using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Domain.Common.Repository;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Tools.Option;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public abstract class RepositoryBase<T, TId> : IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : ValueObject
{
    private readonly DmContext _dmContext;

    public RepositoryBase(DmContext dmContext) => _dmContext = dmContext;

    public virtual async Task AddAsync(T aggregate) =>
        await _dmContext.Set<T>().AddAsync(aggregate);

    public virtual async Task<Option<T>> GetByIdAsync(TId id) =>
        await _dmContext.Set<T>().FindAsync(id);

    public virtual async Task<Result> RemoveAsync(TId id)
    {
        var root = await _dmContext.Set<T>().FindAsync(id);

        if (root is null)
        {
            return Error.NotFound($"{typeof(T).Name}.NotFound", $"Aggregate root with id {id} not found.");
        }

        _dmContext.Set<T>().Remove(root);

        return Result.Success();
    }
}
