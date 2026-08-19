using System.Text.Json;

namespace ViaEventAssociation.Core.Tools.ObjectMapper;

internal class ObjectMapper : IObjectMapper
{
    private readonly IServiceProvider _serviceProvider;

    public ObjectMapper(IServiceProvider serviceProvider) =>
        _serviceProvider = serviceProvider;

    public TOutput Map<TOutput>(object input) where TOutput : class
    {
        Type type = typeof(IMappingConfig<,>).MakeGenericType(input.GetType(), typeof(TOutput));
        var mappingConfig = _serviceProvider.GetService(type);

        if (mappingConfig is not null)
        {
            var mapMethod = type.GetMethod(nameof(IMappingConfig<object, object>.Map));
            if (mapMethod is not null)
            {
                return (TOutput)mapMethod.Invoke(mappingConfig, [input])!;
            }
        }

        string jsonInput = JsonSerializer.Serialize(input);
        return JsonSerializer.Deserialize<TOutput>(jsonInput)!;
    }
}
