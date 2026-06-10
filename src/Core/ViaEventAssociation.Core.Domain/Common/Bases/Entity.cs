namespace ViaEventAssociation.Core.Domain.Common.Bases;

public abstract class Entity<T> where T : notnull
{
    // TODO: Consider making this private
    public T Id { get; }

    protected Entity(T id) => Id = id;

    public override bool Equals(object? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;

        Entity<T> entity = (Entity<T>)other;

        return entity.Id.Equals(Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
