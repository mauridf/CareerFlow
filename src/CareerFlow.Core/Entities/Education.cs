using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa formação acadêmica.
/// </summary>
public class Education : AggregateRoot<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public string Institution { get; private set; } = string.Empty;
    public string Course { get; private set; } = string.Empty;
    public EducationLevel EducationLevel { get; private set; }
    public EducationStatus Status { get; private set; } = EducationStatus.Completed;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? Description { get; private set; }
    public string? Grade { get; private set; }
    public string? ThesisTitle { get; private set; }
    public int DisplayOrder { get; private set; }

    private Education() { }

    public static Education Create(
        Guid personId,
        string institution,
        string course,
        EducationLevel level,
        DateTime startDate,
        DateTime? endDate,
        EducationStatus? status = null,
        string? description = null,
        string? grade = null,
        string? thesisTitle = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(institution))
            throw new ArgumentException("Instituição é obrigatória", nameof(institution));

        if (string.IsNullOrWhiteSpace(course))
            throw new ArgumentException("Curso é obrigatório", nameof(course));

        if (endDate.HasValue && startDate > endDate.Value)
            throw new ArgumentException("Data de início deve ser anterior à data de término");

        var isCurrent = !endDate.HasValue;
        var educationStatus = status ?? (isCurrent ? EducationStatus.InProgress : EducationStatus.Completed);

        var education = new Education
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Institution = institution.Trim(),
            Course = course.Trim(),
            EducationLevel = level,
            StartDate = startDate,
            EndDate = endDate,
            IsCurrent = isCurrent,
            Status = educationStatus,
            Description = description?.Trim(),
            Grade = grade,
            ThesisTitle = thesisTitle?.Trim(),
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        education.AddDomainEvent(new EducationCreatedEvent(education.Id, personId, institution.Trim(), course.Trim(), level));

        return education;
    }

    public void Update(
        string institution,
        string course,
        EducationLevel level,
        DateTime startDate,
        DateTime? endDate,
        EducationStatus status,
        string? description = null,
        string? grade = null,
        string? thesisTitle = null)
    {
        Institution = institution.Trim();
        Course = course.Trim();
        EducationLevel = level;
        StartDate = startDate;
        EndDate = endDate;
        IsCurrent = !endDate.HasValue;
        Status = status;
        Description = description?.Trim();
        Grade = grade;
        ThesisTitle = thesisTitle?.Trim();
        MarkAsUpdated();
        AddDomainEvent(new EducationUpdatedEvent(Id, PersonId));
    }
}
