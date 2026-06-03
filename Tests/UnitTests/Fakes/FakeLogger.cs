using Microsoft.Extensions.Logging;

namespace UnitTests.Fakes;

public class FakeLogger<T> : ILogger<T>
{
    public List<string> Logs { get; } = [];
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => throw new NotImplementedException();
    public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
      Logs.Add(formatter(state, exception));
}
