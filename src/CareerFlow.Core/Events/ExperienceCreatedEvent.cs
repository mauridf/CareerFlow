namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando uma nova experiência profissional é criada.
/// </summary>
public sealed class ExperienceCreatedEvent : DomainEvent
{
    public Guid ExperienceId { get; }
    public Guid PersonId { get; }

    public ExperienceCreatedEvent(Guid experienceId, Guid personId)
    {
        ExperienceId = experienceId;
        PersonId = personId;
    }
}
