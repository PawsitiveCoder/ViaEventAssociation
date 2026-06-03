using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fakes;
using UnitTests.Mocks;
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
            .WithHandler<CreateEventCommand, MockCommandHandler<CreateEventCommand>>()
            .Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var handlerMock = (MockCommandHandler<CreateEventCommand>)serviceProvider.GetService<ICommandHandler<CreateEventCommand>>()!;
        var command = CreateEventCommand.Create();

        await commandDispatcher.DispatchAsync(command.Value);

        Assert.Equal(1, handlerMock.InvokeCount);
        Assert.Equal(command.Value.Id, handlerMock?.Command?.Id);
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
            .WithHandler<CreateEventCommand, MockCommandHandler<CreateEventCommand>>()
            .WithHandler<FakeCommand, FakeHandler>()
            .Build();
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        var handlerMock = (MockCommandHandler<CreateEventCommand>)serviceProvider.GetService<ICommandHandler<CreateEventCommand>>()!;
        var command = CreateEventCommand.Create();

        await commandDispatcher.DispatchAsync(command.Value);

        Assert.True(handlerMock.WasInvoked);
        Assert.Equal(1, handlerMock.InvokeCount);
        Assert.Equal(command.Value.Id, handlerMock?.Command?.Id);
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
