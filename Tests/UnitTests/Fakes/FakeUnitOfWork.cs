using ViaEventAssociation.Core.Domain.Common.UnitOfWork;

namespace UnitTests.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync() => Task.CompletedTask;
}