using JetBrains.Annotations;
using UnitTests.Fakes;
using UnitTests.Mocks;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

[TestSubject(typeof(TimingCommandDispatcher))]
public class TimingCommandDispatcherTests
{
    private class TestCommand1;
    private class TestCommand2;

    [Fact]
    public async Task DispatchAsync_CallsNextDispatcher()
    {
        var nextDispatcher = new MockCommandDispatcher();
        var logger = new FakeLogger<TimingCommandDispatcher>();
        var dispatcher = new TimingCommandDispatcher(nextDispatcher, logger);
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
        var logger = new FakeLogger<TimingCommandDispatcher>();
        var dispatcher = new TimingCommandDispatcher(nextDispatcher, logger);
        var command = new TestCommand1();

        var result = await dispatcher.DispatchAsync(command);

        Assert.Same(expectedResult, result);
    }

    [Fact]
    public async Task DispatchAsync_ReturnsSuccessResult_WhenNextDispatcherReturnsSuccess()
    {
        var nextDispatcher = new MockCommandDispatcher(new Result());
        var logger = new FakeLogger<TimingCommandDispatcher>();
        var dispatcher = new TimingCommandDispatcher(nextDispatcher, logger);
        var command = new TestCommand1();

        var result = await dispatcher.DispatchAsync(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DispatchAsync_ReturnsFailureResult_WhenNextDispatcherReturnsFail()
    {
        var error = Error.Failure("test error", "test details");
        var nextDispatcher = new MockCommandDispatcher(new Result(error));
        var logger = new FakeLogger<TimingCommandDispatcher>();
        var dispatcher = new TimingCommandDispatcher(nextDispatcher, logger);
        var command = new TestCommand1();

        var result = await dispatcher.DispatchAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task DispatchAsync_WithMultipleCommands_LogsEachCommand()
    {
        var nextDispatcher = new MockCommandDispatcher(new Result());
        var logger = new FakeLogger<TimingCommandDispatcher>();
        var dispatcher = new TimingCommandDispatcher(nextDispatcher, logger);
        var command1 = new TestCommand1();
        var command2 = new TestCommand2();

        await dispatcher.DispatchAsync(command1);
        await dispatcher.DispatchAsync(command2);

        Assert.Equal(2, logger.Logs.Count);
        Assert.Contains(nameof(TestCommand1), logger.Logs[0]);
        Assert.Contains(nameof(TestCommand2), logger.Logs[1]);
    }

    [Fact]
    public async Task DispatchAsync_LogsExecutionTime()
    {
        var nextDispatcher = new MockCommandDispatcher(new Result());
        var logger = new FakeLogger<TimingCommandDispatcher>();
        var dispatcher = new TimingCommandDispatcher(nextDispatcher, logger);
        var command = new TestCommand1();

        await dispatcher.DispatchAsync(command);

        var logEntry = Assert.Single(logger.Logs);
        Assert.Contains(nameof(TestCommand1), logEntry);
        Assert.Contains("ms", logEntry);
    }
}
