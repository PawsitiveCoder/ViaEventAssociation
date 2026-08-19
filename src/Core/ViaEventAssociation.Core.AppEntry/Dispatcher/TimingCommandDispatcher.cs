using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.AppEntry;

internal class TimingCommandDispatcher : ICommandDispatcher
{
    private readonly ICommandDispatcher _next;
    private readonly ILogger<TimingCommandDispatcher> _logger;

    public TimingCommandDispatcher(ICommandDispatcher next, ILogger<TimingCommandDispatcher> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Stopwatch stopwatch = new();

        stopwatch.Start();
        Result result = await _next.DispatchAsync(command);
        stopwatch.Stop();

        _logger.LogInformation("Command {CommandType} executed in {ElapsedMilliseconds} ms", typeof(TCommand).Name, stopwatch.Elapsed.TotalMilliseconds);

        return result;
    }
}
