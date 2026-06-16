using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class SetMaxNumberOfGuestsEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenDispatchSucceeds_ReturnsNoContent()
    {
        var endpoint = new SetMaxNumberOfGuestsEndpoint(new MockCommandDispatcher(Result.Success()));

        var result = await endpoint.HandleAsync(new SetMaxNumberOfGuestsRequest(Guid.NewGuid().ToString(), 25));

        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        var endpoint = new SetMaxNumberOfGuestsEndpoint(new MockCommandDispatcher(Result.Success()));

        var result = await endpoint.HandleAsync(new SetMaxNumberOfGuestsRequest(Guid.NewGuid().ToString(), 0));

        Assert.IsType<BadRequest<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsNotFound()
    {
        var endpoint = new SetMaxNumberOfGuestsEndpoint(
            new MockCommandDispatcher(Result.Failure(Error.NotFound("not-found", "Event not found"))));

        var result = await endpoint.HandleAsync(new SetMaxNumberOfGuestsRequest(Guid.NewGuid().ToString(), 25));

        Assert.IsType<NotFound<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new SetMaxNumberOfGuestsEndpoint(new ThrowingCommandDispatcher());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            endpoint.HandleAsync(new SetMaxNumberOfGuestsRequest(Guid.NewGuid().ToString(), 25)));
    }

    private sealed class ThrowingCommandDispatcher : MockCommandDispatcher
    {
        public override Task<Result> DispatchAsync<TCommand>(TCommand command) =>
            throw new InvalidOperationException("boom");
    }
}
