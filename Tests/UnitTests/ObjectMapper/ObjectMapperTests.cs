using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Tools.ObjectMapper;

namespace UnitTests.ObjectMapper;

public class ObjectMapperTests
{
    [Fact]
    public void Map_WithoutSpecificConfig_UsesDefaultJsonMapping()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        IObjectMapper mapper = scope.ServiceProvider.GetRequiredService<IObjectMapper>();

        var input = new DefaultSource("event-id", "Board Games");

        var result = mapper.Map<DefaultTarget>(input);

        Assert.Equal("event-id", result.Id);
        Assert.Equal("Board Games", result.Title);
    }

    [Fact]
    public void Map_WithSpecificConfig_UsesConfigInsteadOfDefaultMapping()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<SpecificSource, SpecificTarget>, SpecificSourceToTargetConfig>()
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        IObjectMapper mapper = scope.ServiceProvider.GetRequiredService<IObjectMapper>();

        var input = new SpecificSource("event-id", "board games");

        var result = mapper.Map<SpecificTarget>(input);

        Assert.Equal("event-id", result.Id);
        Assert.Equal("BOARD GAMES", result.NormalizedTitle);
    }

    [Fact]
    public void Map_WithoutSpecificConfig_MapsComplexObjectGraph()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        IObjectMapper mapper = scope.ServiceProvider.GetRequiredService<IObjectMapper>();

        var input = new ComplexSource(
            "event-id",
            new OrganizerSource("Alicja", "alicia@example.com"),
            ["board-games", "cozy"]);

        var result = mapper.Map<ComplexTarget>(input);

        Assert.Equal("event-id", result.Id);
        Assert.Equal("Alicja", result.Organizer.Name);
        Assert.Equal("alicia@example.com", result.Organizer.Email);
        Assert.Equal(2, result.Tags.Count);
        Assert.Equal("board-games", result.Tags[0]);
        Assert.Equal("cozy", result.Tags[1]);
    }

    [Fact]
    public void Map_WithSpecificConfig_MapsCollectionUsingCustomRules()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IObjectMapper, ViaEventAssociation.Core.Tools.ObjectMapper.ObjectMapper>()
            .AddScoped<IMappingConfig<CollectionSource, CollectionTarget>, CollectionSourceToTargetConfig>()
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        IObjectMapper mapper = scope.ServiceProvider.GetRequiredService<IObjectMapper>();

        var input = new CollectionSource(
            "event-id",
            [
                new GuestSource("  ann@example.com "),
                new GuestSource("BOB@example.com")
            ]);

        var result = mapper.Map<CollectionTarget>(input);

        Assert.Equal("event-id", result.Id);
        Assert.Equal(2, result.Guests.Count);
        Assert.Equal("ann@example.com", result.Guests[0]);
        Assert.Equal("bob@example.com", result.Guests[1]);
    }

    private sealed record DefaultSource(string Id, string Title);
    private sealed record DefaultTarget(string Id, string Title);

    private sealed record SpecificSource(string Id, string Title);
    private sealed record SpecificTarget(string Id, string NormalizedTitle);

    private sealed record OrganizerSource(string Name, string Email);
    private sealed record OrganizerTarget(string Name, string Email);

    private sealed record ComplexSource(string Id, OrganizerSource Organizer, List<string> Tags);
    private sealed record ComplexTarget(string Id, OrganizerTarget Organizer, List<string> Tags);

    private sealed record GuestSource(string Email);
    private sealed record CollectionSource(string Id, List<GuestSource> Guests);
    private sealed record CollectionTarget(string Id, List<string> Guests);

    private sealed class SpecificSourceToTargetConfig : IMappingConfig<SpecificSource, SpecificTarget>
    {
        public SpecificTarget Map(SpecificSource input) =>
            new(input.Id, input.Title.ToUpperInvariant());
    }

    private sealed class CollectionSourceToTargetConfig : IMappingConfig<CollectionSource, CollectionTarget>
    {
        public CollectionTarget Map(CollectionSource input) =>
            new(
                input.Id,
                input.Guests
                    .Select(g => g.Email.Trim().ToLowerInvariant())
                    .ToList());
    }
}
