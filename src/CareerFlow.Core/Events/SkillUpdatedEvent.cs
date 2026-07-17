namespace CareerFlow.Core.Events;

public sealed class SkillUpdatedEvent : DomainEvent
{
    public Guid SkillId { get; }
    public Guid PersonId { get; }

    public SkillUpdatedEvent(Guid skillId, Guid personId)
    {
        SkillId = skillId;
        PersonId = personId;
    }
}
