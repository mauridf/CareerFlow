namespace CareerFlow.Core.Events;

public sealed class SkillCreatedEvent : DomainEvent
{
    public Guid SkillId { get; }
    public Guid PersonId { get; }
    public string Name { get; }

    public SkillCreatedEvent(Guid skillId, Guid personId, string name)
    {
        SkillId = skillId;
        PersonId = personId;
        Name = name;
    }
}
