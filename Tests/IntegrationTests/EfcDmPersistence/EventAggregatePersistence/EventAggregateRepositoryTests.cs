namespace IntegrationTests.EfcDmPersistence.EventAggregatePersistence;

[TestSubject(typeof(EventAggregateRepository))]
public class EventAggregateRepositoryTests
{
    [Fact]
    public async Task AddAsync_WhenCommitted_LoadsSameEventAggregateById()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var repository = new EventAggregateRepository(ctx);
        var unitOfWork = new UnitOfWork(ctx);
        var id = EventId.Create().Value;
        var eventAggregate = EventAggregate.Create(id).Value;

        await repository.AddAsync(eventAggregate);
        await unitOfWork.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var retrieved = await repository.GetByIdAsync(id);
        var aggregate = retrieved.Match<EventAggregate?>(
            onSome: aggregate => aggregate,
            onNone: () => null);
        Assert.NotNull(aggregate);
        Assert.Equal(id, aggregate.Id);
        Assert.Equal(EventStatus.Draft, aggregate.Status);
        Assert.Equal(MaxNumberOfGuests.Create().Value, aggregate.MaxNumberOfGuests);
        Assert.Equal(EventTitle.Create().Value, aggregate.Title);
        Assert.Equal(EventDescription.Create().Value, aggregate.Description);
        Assert.Null(aggregate.TimeInterval);
        Assert.Equal(EventVisibility.Private, aggregate.Visibility);
    }

    [Fact]
    public async Task AddAsync_WhenEventAggregateHasChangedValueObjects_LoadsSameStateAndValueObjects()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var repository = new EventAggregateRepository(ctx);
        var unitOfWork = new UnitOfWork(ctx);
        var id = EventId.Create().Value;
        var title = EventTitle.Create("Repository test event").Value;
        var description = EventDescription.Create("Saved and loaded through the repository.").Value;
        var maxGuests = MaxNumberOfGuests.Create(25).Value;
        var now = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var startDateTime = new DateTime(2026, 1, 1, 11, 0, 0, DateTimeKind.Utc);
        var endDateTime = new DateTime(2026, 1, 1, 13, 0, 0, DateTimeKind.Utc);
        var eventAggregate = EventAggregate.Create(id).Value;
        var updateTitleResult = eventAggregate.UpdateTitle(title);
        var updateDescriptionResult = eventAggregate.UpdateDescription(description);
        var setMaxGuestsResult = eventAggregate.SetMaxNumberOfGuests(maxGuests);
        var updateTimeIntervalResult = eventAggregate.UpdateTimeInterval(startDateTime, endDateTime, now);
        var makePublicResult = eventAggregate.MarkAsPublic();

        Assert.True(updateTitleResult.IsSuccess);
        Assert.True(updateDescriptionResult.IsSuccess);
        Assert.True(setMaxGuestsResult.IsSuccess);
        Assert.True(updateTimeIntervalResult.IsSuccess);
        Assert.True(makePublicResult.IsSuccess);

        await repository.AddAsync(eventAggregate);
        await unitOfWork.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var retrieved = await repository.GetByIdAsync(id);
        var aggregate = retrieved.Match<EventAggregate?>(
            onSome: aggregate => aggregate,
            onNone: () => null);
        Assert.NotNull(aggregate);
        Assert.Equal(id, aggregate.Id);
        Assert.Equal(EventStatus.Draft, aggregate.Status);
        Assert.Equal(maxGuests, aggregate.MaxNumberOfGuests);
        Assert.Equal(title, aggregate.Title);
        Assert.Equal(description, aggregate.Description);
        Assert.NotNull(aggregate.TimeInterval);
        Assert.Equal(startDateTime, aggregate.TimeInterval.StartDateTime);
        Assert.Equal(endDateTime, aggregate.TimeInterval.EndDateTime);
        Assert.Equal(EventVisibility.Public, aggregate.Visibility);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEventAggregateDoesNotExist_ReturnsNone()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var repository = new EventAggregateRepository(ctx);
        var id = EventId.Create().Value;

        var retrieved = await repository.GetByIdAsync(id);

        Assert.True(retrieved.IsNone);
    }

    [Fact]
    public async Task RemoveAsync_WhenCommitted_RemovesEventAggregate()
    {
        await using DmContext ctx = DmContextHelpers.SetupContext();
        var repository = new EventAggregateRepository(ctx);
        var unitOfWork = new UnitOfWork(ctx);
        var id = EventId.Create().Value;
        var eventAggregate = EventAggregate.Create(id).Value;

        await repository.AddAsync(eventAggregate);
        await unitOfWork.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var result = await repository.RemoveAsync(id);
        await unitOfWork.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var retrieved = await repository.GetByIdAsync(id);
        Assert.True(result.IsSuccess);
        Assert.True(retrieved.IsNone);
    }

}
