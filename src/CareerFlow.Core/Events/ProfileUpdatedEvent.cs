namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando um perfil é atualizado.
/// </summary>
public sealed class ProfileUpdatedEvent : DomainEvent
{
    public Guid PersonId { get; }

    public ProfileUpdatedEvent(Guid personId)
    {
        PersonId = personId;
    }
}
