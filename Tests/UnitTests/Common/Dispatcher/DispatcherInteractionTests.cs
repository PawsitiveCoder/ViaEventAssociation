using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fakes;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.AppEntry.Dispatcher;
using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

[TestSubject(typeof(CommandDispatcher))]
public class DispatcherInteractionTests
{
    [Fact]
    public async Task DispatchAsync_NoHandlers_ThrowException()
    {
        var serviceProvider = ServiceBuilder.Init().Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var command = CreateEventCommand.Create();

        await Assert.ThrowsAsync<InvalidOperationException>(() => commandDispatcher.DispatchAsync(command.Value));
    }

    [Fact]
    public async Task DispatchAsync_OneCorrectHandler_Success()
    {
        var serviceProvider = ServiceBuilder.Init()
            .WithHandler<CreateEventCommand, CreateEventHandlerMock>()
            .Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var handlerMock = (CreateEventHandlerMock)serviceProvider.GetService<ICommandHandler<CreateEventCommand>>()!;
        var command = CreateEventCommand.Create();

        await commandDispatcher.DispatchAsync(command.Value);

        Assert.Equal(1, handlerMock.invokeCount);
        Assert.Equal(command.Value.Id, handlerMock.CreateEventCommand.Id);
    }

    [Fact]
    public async Task DispatchAsync_OneIncorrectHandler_ThrowsException()
    {
        var serviceProvider = ServiceBuilder.Init()
            .WithHandler<FakeCommand, FakeHandler>()
            .Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var command = CreateEventCommand.Create();

        await Assert.ThrowsAsync<InvalidOperationException>(() => commandDispatcher.DispatchAsync(command.Value));
    }

    [Fact]
    public async Task DispatchAsync_ManyHandlersWithCorrectHandler_Success()
    {
        var serviceProvider = ServiceBuilder.Init()
            .WithHandler<CreateEventCommand, CreateEventHandlerMock>()
            .WithHandler<FakeCommand, FakeHandler>()
            .Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var handlerMock = (CreateEventHandlerMock)serviceProvider.GetService<ICommandHandler<CreateEventCommand>>()!;
        var command = CreateEventCommand.Create();

        await commandDispatcher.DispatchAsync(command.Value);

        Assert.True(handlerMock.wasInvoked);
        Assert.Equal(1, handlerMock.invokeCount);
        Assert.Equal(command.Value.Id, handlerMock.CreateEventCommand.Id);
    }

    [Fact]
    public async Task DispatchAsync_ManyHandlersWithoutCorrectHandler_ThrowsException()
    {
        var serviceProvider = ServiceBuilder.Init()
            .WithHandler<UpdateEventTitleCommand, UpdateEventTitleHandler>()
            .WithHandler<FakeCommand, FakeHandler>()
            .Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var command = CreateEventCommand.Create();

        await Assert.ThrowsAsync<InvalidOperationException>(() => commandDispatcher.DispatchAsync(command.Value));
    }
}
