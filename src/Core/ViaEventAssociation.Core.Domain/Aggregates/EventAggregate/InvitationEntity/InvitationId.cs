using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.InvitationEntity;

public class InvitationId
{
    public Guid Value { get; set; }

    protected InvitationId()
    {
    }
    public static Result<InvitationId> Create(Guid id)
    {
        Value = id;
        return this;
    }
}
