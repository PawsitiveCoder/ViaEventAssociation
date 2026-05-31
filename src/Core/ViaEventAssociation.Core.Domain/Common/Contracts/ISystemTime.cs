namespace ViaEventAssociation.Core.Domain.Common.Contracts;

public interface ISystemTime
{
    DateTime UtcNow { get; }
}