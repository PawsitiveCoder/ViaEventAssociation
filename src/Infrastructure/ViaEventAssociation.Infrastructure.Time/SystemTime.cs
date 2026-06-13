using ViaEventAssociation.Core.Domain.Common.Time;

namespace ViaEventAssociation.Infrastructure.Time;

public class SystemTime : ISystemTime
{
    public DateTime CurrentTime() => DateTime.UtcNow;
}
