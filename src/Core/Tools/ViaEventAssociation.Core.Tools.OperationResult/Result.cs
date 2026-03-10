namespace ViaEventAssociation.Core.Tools.OperationResult;

public class Result<T>
{
    public T Value { get; set; }
    public Error? Error { get; private set; }
    public List<Error> Errors { get; set; }
    
    public Result(T value) => Value = value;
    public Result(Error error) => Error = error;
    public Result(List<Error> errors) => Errors = errors;

    public bool IsSuccess => Error is null;
    
    public bool HasErrors => !IsSuccess;
    
    public static implicit operator Result<T>(Error error) => new(error);
    
    public static implicit operator Result<T>(T value) => new(value);

    public static Result<T> FromResult<T2>(Result<T2> result) => new(result.Errors);

    public Result<T> WithResult<T2>(Result<T2> result)
    {
        result.Errors.AddRange(result.Errors);
        return this;
    }
}

public class Result
{
    public Error? Error { get; private set; }
 
    public Result() {}
    public Result(Error error) => Error = error;
    
    public bool IsSuccess => Error is null;
    
    public bool HasErrors => !IsSuccess;

    public static implicit operator Result(Error error) => new(error);
    
    public static Result<TV> Failure<TV>(Error error) => new(error);
    
    public static Result Failure(Error error) => new(error);
    
    public static Result<TV> Success<TV>(TV value) => new(value);

    public static Result Success() => new();
}
