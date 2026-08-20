namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Result<T> : Result
{
    public T? Payload { get; }
    public T Value => Payload!;

    public Result() { }
    public Result(T payload) => Payload = payload;
    public Result(Error error) : base(error) { }
    public Result(IEnumerable<Error> errors) : base(errors) { }

    public static implicit operator Result<T>(Error error) => new(error);

    public static implicit operator Result<T>(T payload) => new(payload);

    public static Result<T> FromResult<T2>(Result<T2> result) =>
        result.HasErrors ? new Result<T>(result.Errors) : new Result<T>();

    public Result<T> WithResult<T2>(Result<T2> result) =>
        result.HasErrors
            ? new Result<T>(Errors.Concat(result.Errors))
            : this;

    public Result<T> WithPayloadIfSuccess(Func<T> payloadFactory) =>
        HasErrors ? new Result<T>(Errors) : new Result<T>(payloadFactory());
}

public class Result
{
    private readonly List<Error> _errors;

    public IReadOnlyList<Error> Errors { get; }

    public Error? Error => Errors.Count > 0 ? Errors[0] : null;

    public Result()
    {
        _errors = [];
        Errors = _errors.AsReadOnly();
    }

    public Result(Error error) : this() => _errors.Add(error);

    public Result(IEnumerable<Error> errors) : this() => _errors.AddRange(errors);

    public bool IsSuccess => Errors.Count == 0;

    public bool HasErrors => !IsSuccess;

    public static implicit operator Result(Error error) => new(error);

    public static implicit operator Task<Result>(Result result) => Task.FromResult(result);

    public static Result<TValue> CombineResultsInto<TValue>(params Result[] results)
    {
        var errors = new List<Error>();

        foreach (var result in results)
        {
            errors.AddRange(result.Errors);
        }

        return errors.Count > 0 ? new Result<TValue>(errors) : new Result<TValue>();
    }

    public static Result<TV> Failure<TV>(Error error) => new(error);

    public static Result Failure(Error error) => new(error);

    public static Result<TV> Success<TV>(TV value) => new(value);

    public static Result Success() => new();
}
