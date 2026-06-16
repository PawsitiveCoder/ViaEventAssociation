using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class MakeEventPublicEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenDispatchSucceeds_ReturnsNoContent()
    {
        var endpoint = new MakeEventPublicEndpoint(new MockCommandDispatcher(Result.Success()));

        var result = await endpoint.HandleAsync(new MakeEventPublicRequest(Guid.NewGuid().ToString()));

        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenBusinessRuleFails_ReturnsBadRequest()
    {
        var endpoint = new MakeEventPublicEndpoint(
            new MockCommandDispatcher(Result.Failure(Error.Validation("rule-violation", "Cannot make event public"))));

        var result = await endpoint.HandleAsync(new MakeEventPublicRequest(Guid.NewGuid().ToString()));

        Assert.IsType<BadRequest<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsNotFound()
    {
        var endpoint = new MakeEventPublicEndpoint(
            new MockCommandDispatcher(Result.Failure(Error.NotFound("not-found", "Event not found"))));

        var result = await endpoint.HandleAsync(new MakeEventPublicRequest(Guid.NewGuid().ToString()));

        Assert.IsType<NotFound<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new MakeEventPublicEndpoint(new ThrowingCommandDispatcher());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            endpoint.HandleAsync(new MakeEventPublicRequest(Guid.NewGuid().ToString())));
    }

    private sealed class ThrowingCommandDispatcher : MockCommandDispatcher
    {
        public override Task<Result> DispatchAsync<TCommand>(TCommand command) =>
            throw new InvalidOperationException("boom");
    }
}
