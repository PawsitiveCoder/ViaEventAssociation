using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class UpdateEventTitleEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        var endpoint = new UpdateEventTitleEndpoint(new MockCommandDispatcher(Result.Success()));

        var result = await endpoint.HandleAsync(new UpdateEventTitleRequest(Guid.NewGuid().ToString(), ""));

        Assert.IsType<BadRequest<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsNotFound()
    {
        var endpoint = new UpdateEventTitleEndpoint(
            new MockCommandDispatcher(Result.Failure(Error.NotFound("not-found", "missing"))));

        var result = await endpoint.HandleAsync(new UpdateEventTitleRequest(Guid.NewGuid().ToString(), "Title"));

        Assert.IsType<NotFound<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new UpdateEventTitleEndpoint(new ThrowingCommandDispatcher());

        await Assert.ThrowsAsync<InvalidOperationException>(() => endpoint.HandleAsync(new UpdateEventTitleRequest(Guid.NewGuid().ToString(), "Title")));
    }

    private sealed class ThrowingCommandDispatcher : MockCommandDispatcher
    {
        public override Task<Result> DispatchAsync<TCommand>(TCommand command) =>
            throw new InvalidOperationException("boom");
    }
}
