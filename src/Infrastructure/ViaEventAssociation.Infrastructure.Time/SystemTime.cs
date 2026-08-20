using ViaEventAssociation.Core.Domain.Common.Time;

namespace ViaEventAssociation.Infrastructure.Time;

internal class SystemTime : ISystemTime
{
    public DateTime CurrentTime() => DateTime.UtcNow;
}
