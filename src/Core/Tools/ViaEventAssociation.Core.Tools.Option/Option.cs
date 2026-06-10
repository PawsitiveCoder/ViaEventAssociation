namespace ViaEventAssociation.Core.Tools.Option;

public record Option<T>
{
    private readonly T? _value;
    public bool IsSome { get; }
    public bool IsNone => !IsSome;

    private Option(T? value, bool isSome)
    {
        _value = value;
        IsSome = isSome;
    }

    public static Option<T> Some(T value) =>
        value is null ? None() : new(value, true);

    public static Option<T> None() => new(default, false);

    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) =>
        IsSome ? onSome(_value!) : onNone();

    public static implicit operator Option<T>(T? value) =>
        value is null ? None() : Some(value);
}
