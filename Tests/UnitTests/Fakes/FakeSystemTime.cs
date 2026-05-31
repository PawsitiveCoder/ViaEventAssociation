using ViaEventAssociation.Core.Domain.Common.Contracts;

namespace UnitTests.Fakes;

public class FakeSystemTime : ISystemTime
{
    public DateTime UtcNow { get; set; }
}