using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.AppEntry;
using ViaEventAssociation.Core.Application;


namespace UnitTests.Common.Services;

[TestSubject(typeof(DependencyInjection))]
public class DependencyInjectionUnitTests
{
    [Theory]
    [InlineData(typeof(ICommandHandler<CreateEventCommand>))]
    [InlineData(typeof(ICommandHandler<UpdateEventTitleCommand>))]
    public void AddApplicationCommandHandlers_ShouldRegisterCommandHandlers(Type serviceType)
    {
        var services = new ServiceCollection();

        services.AddApplicationCommandHandlers();

        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == serviceType);
        Assert.NotNull(serviceDescriptor);
    }
}
