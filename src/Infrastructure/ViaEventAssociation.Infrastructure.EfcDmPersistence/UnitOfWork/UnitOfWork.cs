using ViaEventAssociation.Core.Domain.Common.UnitOfWork;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DmContext _dmContext;

    public UnitOfWork(DmContext dmContext) => _dmContext = dmContext;

    public Task SaveChangesAsync() => _dmContext.SaveChangesAsync();
}
