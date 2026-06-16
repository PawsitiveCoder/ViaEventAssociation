using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Domain.Common.Time;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class UpdateEventTimeIntervalEndpointTests
{
    private static readonly ISystemTime FakeTime = new FakeSystemTime();

    [Fact]
    public async Task HandleAsync_WhenDispatchSucceeds_ReturnsNoContent()
    {
        var endpoint = new UpdateEventTimeIntervalEndpoint(new MockCommandDispatcher(Result.Success()), FakeTime);
        var request = new UpdateEventTimeIntervalRequest(
            Guid.NewGuid().ToString(),
            new DateTime(2027, 6, 15, 10, 0, 0),
            new DateTime(2027, 6, 15, 12, 0, 0));

        var result = await endpoint.HandleAsync(request);

        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenTimeIntervalIsInvalid_ReturnsBadRequest()
    {
        var endpoint = new UpdateEventTimeIntervalEndpoint(new MockCommandDispatcher(Result.Success()), FakeTime);
        var request = new UpdateEventTimeIntervalRequest(
            Guid.NewGuid().ToString(),
            new DateTime(2020, 1, 1, 10, 0, 0),
            new DateTime(2020, 1, 1, 12, 0, 0));

        var result = await endpoint.HandleAsync(request);

        Assert.IsType<BadRequest<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsNotFound()
    {
        var endpoint = new UpdateEventTimeIntervalEndpoint(
            new MockCommandDispatcher(Result.Failure(Error.NotFound("not-found", "Event not found"))),
            FakeTime);
        var request = new UpdateEventTimeIntervalRequest(
            Guid.NewGuid().ToString(),
            new DateTime(2027, 6, 15, 10, 0, 0),
            new DateTime(2027, 6, 15, 12, 0, 0));

        var result = await endpoint.HandleAsync(request);

        Assert.IsType<NotFound<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new UpdateEventTimeIntervalEndpoint(new ThrowingCommandDispatcher(), FakeTime);
        var request = new UpdateEventTimeIntervalRequest(
            Guid.NewGuid().ToString(),
            new DateTime(2027, 6, 15, 10, 0, 0),
            new DateTime(2027, 6, 15, 12, 0, 0));

        await Assert.ThrowsAsync<InvalidOperationException>(() => endpoint.HandleAsync(request));
    }

    private sealed class FakeSystemTime : ISystemTime
    {
        public DateTime CurrentTime() => new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    }

    private sealed class ThrowingCommandDispatcher : MockCommandDispatcher
    {
        public override Task<Result> DispatchAsync<TCommand>(TCommand command) =>
            throw new InvalidOperationException("boom");
    }
}
