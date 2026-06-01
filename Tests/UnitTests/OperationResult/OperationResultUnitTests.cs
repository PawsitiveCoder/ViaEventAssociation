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
}
