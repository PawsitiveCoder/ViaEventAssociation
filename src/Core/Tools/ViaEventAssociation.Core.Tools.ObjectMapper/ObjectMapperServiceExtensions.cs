using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public static class ObjectMapperServiceExtensions
{
    public static IServiceCollection AddObjectMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<IObjectMapper, ObjectMapper>();

        var targetAssemblies = assemblies.Length > 0 ? assemblies : [Assembly.GetCallingAssembly()];

        var mappingConfigs = targetAssemblies
            .Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract)
            .SelectMany(type => type.GetInterfaces()
                .Where(IsMappingConfigInterface)
                .Select(service => new { Service = service, Implementation = type }));

        foreach (var mappingConfig in mappingConfigs)
        {
            services.AddScoped(mappingConfig.Service, mappingConfig.Implementation);
        }

        return services;
    }

    private static bool IsMappingConfigInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMappingConfig<,>);
}
