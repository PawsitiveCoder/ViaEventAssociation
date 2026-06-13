using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class CreateEventEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenDispatchSucceeds_ReturnsNoContent()
    {
        var endpoint = new CreateEventEndpoint(new MockCommandDispatcher(Result.Success()));

        var result = await endpoint.HandleAsync();

        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatchReturnsError_ReturnsBadRequest()
    {
        var endpoint = new CreateEventEndpoint(new MockCommandDispatcher(Result.Failure(Error.Validation("code", "invalid"))));

        var result = await endpoint.HandleAsync();

        var badRequest = Assert.IsType<BadRequest<List<Error>>>(result.Result);
        Assert.Single(badRequest.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new CreateEventEndpoint(new ThrowingCommandDispatcher());

        await Assert.ThrowsAsync<InvalidOperationException>(endpoint.HandleAsync);
    }

    private sealed class ThrowingCommandDispatcher : MockCommandDispatcher
    {
        public override Task<Result> DispatchAsync<TCommand>(TCommand command) =>
            throw new InvalidOperationException("boom");
    }
}
