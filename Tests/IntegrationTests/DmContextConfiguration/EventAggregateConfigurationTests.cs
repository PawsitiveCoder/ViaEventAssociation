using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace IntegrationTests.DmContextConfiguration;

[TestSubject(typeof(EventAggregateConfiguration))]
public class EventAggregateConfigurationTests
{
    [Fact]
    public async Task EventAggregate_CanBePersistedAndFoundByStrongId()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.SingleOrDefault(e => e.Id.Equals(idResult.Value));
        Assert.NotNull(retrieved);
    }

    [Fact]
    public async Task EventAggregateWithTitle_CanBePersistedAndRehydrated()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var titleResult = EventTitle.Create("Test title");
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);
        eventAggregateResult.Value.UpdateTitle(titleResult.Value);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.NotNull(retrieved.Title);
        Assert.Equal(titleResult.Value, retrieved.Title);
    }

    [Fact]
    public async Task EventAggregateWithDescription_CanBePersistedAndRehydrated()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var descriptionResult = EventDescription.Create("Test description");
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);
        eventAggregateResult.Value.UpdateDescription(descriptionResult.Value);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.NotNull(retrieved.Description);
        Assert.Equal(descriptionResult.Value, retrieved.Description);
    }

    [Fact]
    public async Task EventAggregateWithMaxNumberOfGuests_CanBePersistedAndRehydrated()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var maxNumberOfGuestsResult = MaxNumberOfGuests.Create(25);
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);
        eventAggregateResult.Value.SetMaxNumberOfGuests(maxNumberOfGuestsResult.Value);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.NotNull(retrieved.MaxNumberOfGuests);
        Assert.Equal(maxNumberOfGuestsResult.Value, retrieved.MaxNumberOfGuests);
    }

    [Fact]
    public async Task EventAggregateWithDefaultStatus_CanBePersistedAndRehydrated()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.NotNull(retrieved.Status);
        Assert.Equal(EventStatus.Draft, retrieved.Status);
    }

    [Fact]
    public async Task EventAggregateWithTimeInterval_CanBePersistedAndRehydrated()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);
        var now = DateTime.UtcNow;
        var startDateTime = now.AddHours(1);
        var endDateTime = now.AddHours(2);
        var timeIntervalResult = TimeInterval.Create(startDateTime, endDateTime, now);
        eventAggregateResult.Value.UpdateTimeInterval(startDateTime, endDateTime, now);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.NotNull(retrieved.TimeInterval);
        Assert.Equal(timeIntervalResult.Value.StartDateTime, retrieved.TimeInterval.StartDateTime);
        Assert.Equal(timeIntervalResult.Value.EndDateTime, retrieved.TimeInterval.EndDateTime);
    }

    [Fact]
    public async Task EventAggregateWithoutTimeInterval_IsRehydratedWithNoTimeInterval()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.Null(retrieved.TimeInterval);
    }

    [Fact]
    public async Task PublicEventAggregate_CanBePersistedAndRehydrated()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var idResult = EventId.Create();
        var eventAggregateResult = EventAggregate.Create(idResult.Value);
        eventAggregateResult.Value.MarkAsPublic();

        await DmContextHelpers.SaveAndClearAsync(eventAggregateResult.Value, ctx);

        var retrieved = ctx.EventAggregates.Single(e => e.Id.Equals(idResult.Value));
        Assert.Equal(EventVisibility.Public, retrieved.Visibility);
    }
}
