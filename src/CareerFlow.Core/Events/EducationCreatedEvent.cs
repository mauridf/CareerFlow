using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Events;

public sealed class EducationCreatedEvent : DomainEvent
{
    public Guid EducationId { get; }
    public Guid PersonId { get; }
    public string Institution { get; }
    public string Course { get; }
    public EducationLevel Level { get; }

    public EducationCreatedEvent(Guid educationId, Guid personId, string institution, string course, EducationLevel level)
    {
        EducationId = educationId;
        PersonId = personId;
        Institution = institution;
        Course = course;
        Level = level;
    }
}
