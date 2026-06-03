using ViaEventAssociation.Core.Domain.Common.UnitOfWork;

namespace UnitTests.Mocks;

public class MockUnitOfWork : IUnitOfWork
{
    private readonly Exception? _throwException;
    public int InvokeCount { get; private set; } = 0;
    public bool WasInvoked => InvokeCount > 0;

    public MockUnitOfWork(Exception? throwException = null) => _throwException = throwException;

    public Task SaveChangesAsync()
    {
        if (_throwException is not null) throw _throwException;
        InvokeCount++;
        return Task.CompletedTask;
    }
}
