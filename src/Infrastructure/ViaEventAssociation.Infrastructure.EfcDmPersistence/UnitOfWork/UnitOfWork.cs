using ViaEventAssociation.Core.Domain.Common.UnitOfWork;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DmContext _dmContext;

    public UnitOfWork(DmContext dmContext) => _dmContext = dmContext;

    public async Task SaveChangesAsync() => await _dmContext.SaveChangesAsync();
}
