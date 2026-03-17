namespace ViaEventAssociation.Core.Domain.Common.Bases;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(obj => obj?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public override bool Equals(object? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;

        return ((ValueObject)other).GetEqualityComponents()
            .SequenceEqual(GetEqualityComponents());
    }

    public static bool operator ==(ValueObject left, ValueObject right) => Equals(left, right);

    public static bool operator !=(ValueObject left, ValueObject right) => !Equals(left, right);
}
