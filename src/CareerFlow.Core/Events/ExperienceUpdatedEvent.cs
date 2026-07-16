namespace CareerFlow.Core.Events;

/// <summary>
/// Evento disparado quando uma experiência profissional é atualizada.
/// </summary>
public sealed class ExperienceUpdatedEvent : DomainEvent
{
    public Guid ExperienceId { get; }

    public ExperienceUpdatedEvent(Guid experienceId)
    {
        ExperienceId = experienceId;
    }
}
