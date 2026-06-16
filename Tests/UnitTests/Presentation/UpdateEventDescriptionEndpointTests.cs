using Microsoft.AspNetCore.Http.HttpResults;
using ViaEventAssociation.Core.Tools.OperationResult;
using ViaEventAssociation.Presentation.WebApi.Endpoints.VeaEvents;
using UnitTests.Mocks;

namespace UnitTests.Presentation;

public class UpdateEventDescriptionEndpointTests
{
    [Fact]
    public async Task HandleAsync_WhenDispatchSucceeds_ReturnsNoContent()
    {
        var endpoint = new UpdateEventDescriptionEndpoint(new MockCommandDispatcher(Result.Success()));

        var result = await endpoint.HandleAsync(new UpdateEventDescriptionRequest(Guid.NewGuid().ToString(), "A valid description"));

        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDescriptionTooLong_ReturnsBadRequest()
    {
        var endpoint = new UpdateEventDescriptionEndpoint(new MockCommandDispatcher(Result.Success()));
        var tooLongDescription = new string('x', 251);

        var result = await endpoint.HandleAsync(new UpdateEventDescriptionRequest(Guid.NewGuid().ToString(), tooLongDescription));

        Assert.IsType<BadRequest<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsNotFound()
    {
        var endpoint = new UpdateEventDescriptionEndpoint(
            new MockCommandDispatcher(Result.Failure(Error.NotFound("not-found", "Event not found"))));

        var result = await endpoint.HandleAsync(new UpdateEventDescriptionRequest(Guid.NewGuid().ToString(), "A valid description"));

        Assert.IsType<NotFound<List<Error>>>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenDispatcherThrows_Throws()
    {
        var endpoint = new UpdateEventDescriptionEndpoint(new ThrowingCommandDispatcher());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            endpoint.HandleAsync(new UpdateEventDescriptionRequest(Guid.NewGuid().ToString(), "A valid description")));
    }

    private sealed class ThrowingCommandDispatcher : MockCommandDispatcher
    {
        public override Task<Result> DispatchAsync<TCommand>(TCommand command) =>
            throw new InvalidOperationException("boom");
    }
}
