namespace Shared.Fakes;

public sealed class FakeSystemTime : ISystemTime
{
    private readonly DateTime _now;

    public FakeSystemTime(DateTime now) => _now = now;

    public DateTime CurrentTime() => _now;
}
