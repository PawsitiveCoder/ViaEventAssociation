using JetBrains.Annotations;
using UnitTests.Mocks;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.AppEntry.Dispatcher;
using ViaEventAssociation.Core.Domain.Common.UnitOfWork;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

[TestSubject(typeof(TransactionalCommandDispatcher))]
public class TransactionalCommandDispatcherTests
{
    private class TestCommand1;
    private class TestCommand2;

    [Fact]
    public async Task DispatchAsync_CallsNextDispatcher()
    {
        var nextDispatcher = new MockCommandDispatcher();
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command = new TestCommand1();

        await dispatcher.DispatchAsync(command);

        Assert.True(nextDispatcher.WasInvoked);
        Assert.Equal(1, nextDispatcher.InvokeCount);
    }

    [Fact]
    public async Task DispatchAsync_ReturnsResultFromNextDispatcher()
    {
        var expectedResult = new Result();
        var nextDispatcher = new MockCommandDispatcher(expectedResult);
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command = new TestCommand1();

        var result = await dispatcher.DispatchAsync(command);

        Assert.Same(expectedResult, result);
    }

    [Fact]
    public async Task DispatchAsync_CallsSaveChangesAsync_WhenResultIsSuccess()
    {
        var nextDispatcher = new MockCommandDispatcher(new Result());
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command = new TestCommand1();

        await dispatcher.DispatchAsync(command);

        Assert.True(unitOfWork.WasInvoked);
    }

    [Fact]
    public async Task DispatchAsync_DoesNotCallSaveChangesAsync_WhenResultHasErrors()
    {
        var error = Error.Failure("test error", "test details");
        var nextDispatcher = new MockCommandDispatcher(new Result(error));
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command = new TestCommand1();

        await dispatcher.DispatchAsync(command);

        Assert.False(unitOfWork.WasInvoked);
    }

    [Fact]
    public async Task DispatchAsync_ReturnsSuccessResult_WhenNextDispatcherReturnsSuccess()
    {
        var nextDispatcher = new MockCommandDispatcher(new Result());
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command = new TestCommand1();

        var result = await dispatcher.DispatchAsync(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DispatchAsync_ReturnsFailureResult_WhenNextDispatcherReturnsFail()
    {
        var error = Error.Failure("test error", "test details");
        var nextDispatcher = new MockCommandDispatcher(new Result(error));
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command = new TestCommand1();

        var result = await dispatcher.DispatchAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task DispatchAsync_WithMultipleSuccessfulCommands_SavesChangesForEach()
    {
        var nextDispatcher = new MockCommandDispatcher(new Result());
        var unitOfWork = new MockUnitOfWork();
        var dispatcher = new TransactionalCommandDispatcher(nextDispatcher, unitOfWork);
        var command1 = new TestCommand1();
        var command2 = new TestCommand2();

        await dispatcher.DispatchAsync(command1);
        await dispatcher.DispatchAsync(command2);

        Assert.Equal(2, unitOfWork.InvokeCount);
    }
}
