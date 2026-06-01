using JetBrains.Annotations;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace UnitTests.Features.Event.UpdateTitle;

[TestSubject(typeof(UpdateEventTitleCommand))]
public class UpdateEventTitleCommandTests
{
    [Fact]
    public void UpdateEventTitleCommand_ValidInput_Success()
    {
        string id = Guid.NewGuid().ToString();
        string title = "Test title";

        var result = UpdateEventTitleCommand.Create(id, title);
        var command = result.Payload;

        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(id, command.EventId.Value.ToString());
        Assert.Equal(title, command.EventTitle.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData(null)]
    public void UpdateEventTitleCommand_InvalidTitle_Failure(string? invalidTitle)
    {
        string id = Guid.NewGuid().ToString();

        var result = UpdateEventTitleCommand.Create(id, invalidTitle);

        Assert.NotNull(result.Error);
        Assert.Contains($"between {EventTitle.MinLength} and {EventTitle.MaxLength}", result.Error.Description);
    }
}
