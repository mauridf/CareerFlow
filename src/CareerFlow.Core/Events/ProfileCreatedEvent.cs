namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um perfil é criado.
/// </summary>
public sealed class ProfileCreatedEvent : DomainEvent
{
    public Guid PersonId { get; }
    public Guid UserId { get; }

    public ProfileCreatedEvent(Guid personId, Guid userId)
    {
        PersonId = personId;
        UserId = userId;
    }
}
