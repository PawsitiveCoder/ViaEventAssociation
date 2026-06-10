using JetBrains.Annotations;
using ViaEventAssociation.Core.Tools.Option;

namespace UnitTests.Option;

[TestSubject(typeof(Option<>))]
public class OptionUnitTests
{
    [Fact]
    public void Some_WithValue_ReturnsSome()
    {
        var option = Option<string>.Some("value");

        Assert.True(option.IsSome);
        Assert.False(option.IsNone);
    }

    [Fact]
    public void Some_WithNull_ReturnsNone()
    {
        var option = Option<string>.Some(null!);

        Assert.False(option.IsSome);
        Assert.True(option.IsNone);
    }

    [Fact]
    public void None_ReturnsNone()
    {
        var option = Option<string>.None();

        Assert.False(option.IsSome);
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Match_WithSome_ReturnsOnSomeResult()
    {
        var option = Option<string>.Some("value");

        var result = option.Match(
            onSome: value => value.ToUpperInvariant(),
            onNone: () => "none");

        Assert.Equal("VALUE", result);
    }

    [Fact]
    public void Match_WithNone_ReturnsOnNoneResult()
    {
        var option = Option<string>.None();

        var result = option.Match(
            onSome: value => value.ToUpperInvariant(),
            onNone: () => "none");

        Assert.Equal("none", result);
    }

    [Fact]
    public void ImplicitConversion_WithValue_ReturnsSome()
    {
        Option<string> option = "value";

        Assert.True(option.IsSome);
    }

    [Fact]
    public void ImplicitConversion_WithNull_ReturnsNone()
    {
        string? value = null;

        Option<string> option = value;

        Assert.True(option.IsNone);
    }
}
