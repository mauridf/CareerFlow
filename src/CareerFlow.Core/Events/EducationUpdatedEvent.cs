namespace CareerFlow.Core.Events;

public sealed class EducationUpdatedEvent : DomainEvent
{
    public Guid EducationId { get; }
    public Guid PersonId { get; }

    public EducationUpdatedEvent(Guid educationId, Guid personId)
    {
        EducationId = educationId;
        PersonId = personId;
    }
}
