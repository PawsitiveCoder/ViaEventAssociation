using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Tools.Option;

namespace ViaEventAssociation.Core.Domain.Common.Repository;

public interface IGenericRepository<T, in TId>
    where T : AggregateRoot<TId>
    where TId : ValueObject
{
    Task AddAsync(T aggregate);
    Task<Option<T>> GetByIdAsync(TId id);
    Task<Result> RemoveAsync(TId id);
}
