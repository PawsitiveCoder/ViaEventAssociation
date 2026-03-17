namespace ViaEventAssociation.Core.Domain.Common.Bases;

public abstract class Entity<T>
{
    public T Id { get; }

    protected Entity(T id)
    {
        Id = id;
    }

    protected Entity() { }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        Entity<T> entity = (Entity<T>)obj;
        return entity.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
