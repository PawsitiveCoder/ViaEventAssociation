namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public interface IObjectMapper
{
    public TOutput Map<TOutput>(object input) where TOutput : class;
}
