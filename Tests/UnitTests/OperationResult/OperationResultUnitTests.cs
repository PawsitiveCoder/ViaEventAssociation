using JetBrains.Annotations;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.OperationResult;

[TestSubject(typeof(Result<>))]
public class OperationResultUnitTests
{
    [Fact]
    public void CombineResultsInto_WithPayloadIfSuccess_ReturnsPayloadWhenAllResultsSucceed()
    {
        Result first = Result.Success();
        Result second = Result.Success();

        var result = Result
            .CombineResultsInto<string>(first, second)
            .WithPayloadIfSuccess(() => "created");

        Assert.True(result.IsSuccess);
        Assert.Equal("created", result.Payload);
    }

    [Fact]
    public void CombineResultsInto_WithPayloadIfSuccess_ReturnsErrorsWhenAnyResultFails()
    {
        Result first = Result.Success();
        Result second = Error.Validation("test.validation", "Invalid test value.");

        var result = Result
            .CombineResultsInto<string>(first, second)
            .WithPayloadIfSuccess(() => "should not be created");

        Assert.True(result.HasErrors);
        Assert.Null(result.Payload);
        Assert.Equal("test.validation", result.Error!.Code);
    }

    [Fact]
    public void Value_CannotBeReassignedByCaller()
    {
        var valueProperty = typeof(Result<string>).GetProperty(nameof(Result<string>.Value));

        Assert.NotNull(valueProperty);
        Assert.Null(valueProperty.SetMethod);
    }

    [Fact]
    public void Errors_CannotBeMutatedByCaller()
    {
        Result result = Result.Failure(Error.Validation("test.validation", "Invalid test value."));
        var errors = Assert.IsAssignableFrom<IList<Error>>(result.Errors);

        Assert.Throws<NotSupportedException>(() =>
            errors.Add(Error.Failure("test.failure", "Should not be added.")));
        Assert.Single(result.Errors);
    }

    [Fact]
    public void WithResult_ReturnsNewFailureWithoutMutatingOriginalResult()
    {
        var original = Result.Success("created");
        Result<int> failure = Result.Failure<int>(Error.Validation("test.validation", "Invalid test value."));

        var combined = original.WithResult(failure);

        Assert.True(original.IsSuccess);
        Assert.Empty(original.Errors);
        Assert.True(combined.HasErrors);
        Assert.Single(combined.Errors);
    }
}
