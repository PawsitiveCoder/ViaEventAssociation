namespace ViaEventAssociation.Core.Domain.Common.Bases;

public abstract class AggregateRoot<T> : Entity<T> where T : notnull
{
    protected AggregateRoot(T id) : base(id)
    {
    }
}
