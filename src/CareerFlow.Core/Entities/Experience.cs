using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa uma experiência profissional (Aggregate Root).
/// </summary>
public class Experience : AggregateRoot<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public string CompanyName { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? Description { get; private set; }
    public List<Guid> SkillsUsed { get; private set; } = new();
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string Country { get; private set; } = "Brasil";
    public EmploymentType? EmploymentType { get; private set; }
    public int DisplayOrder { get; private set; }

    private Experience() { }

    public static Experience Create(
        Guid personId,
        string companyName,
        string position,
        DateTime startDate,
        DateTime? endDate,
        string? description,
        List<Guid>? skillsUsed = null,
        string? city = null,
        string? state = null,
        string? country = null,
        EmploymentType? employmentType = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Nome da empresa é obrigatório");

        if (string.IsNullOrWhiteSpace(position))
            throw new DomainException("Cargo é obrigatório");

        if (endDate.HasValue && startDate > endDate.Value)
            throw new DomainException("Data de início deve ser anterior à data de término");

        if (!string.IsNullOrWhiteSpace(description) && description.Length < 50)
            throw new DomainException("Descrição deve ter no mínimo 50 caracteres");

        // Valida o período usando o Value Object
        var dateRange = new ValueObjects.DateRange(startDate, endDate);

        var experience = new Experience
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            CompanyName = companyName.Trim(),
            Position = position.Trim(),
            StartDate = dateRange.StartDate,
            EndDate = dateRange.EndDate,
            IsCurrent = dateRange.IsOngoing,
            Description = description?.Trim(),
            SkillsUsed = skillsUsed ?? new List<Guid>(),
            City = city?.Trim(),
            State = state?.Trim()?.ToUpper(),
            Country = country?.Trim() ?? "Brasil",
            EmploymentType = employmentType,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        experience.AddDomainEvent(new ExperienceCreatedEvent(experience.Id, personId));

        return experience;
    }

    public void Update(
        string companyName,
        string position,
        DateTime startDate,
        DateTime? endDate,
        string? description,
        EmploymentType? employmentType = null,
        string? city = null,
        string? state = null,
        List<Guid>? skillsUsed = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Nome da empresa é obrigatório");

        if (string.IsNullOrWhiteSpace(position))
            throw new DomainException("Cargo é obrigatório");

        var dateRange = new ValueObjects.DateRange(startDate, endDate);

        CompanyName = companyName.Trim();
        Position = position.Trim();
        StartDate = dateRange.StartDate;
        EndDate = dateRange.EndDate;
        IsCurrent = dateRange.IsOngoing;
        Description = description?.Trim();
        EmploymentType = employmentType;
        City = city?.Trim();
        State = state?.Trim()?.ToUpper();

        if (skillsUsed != null)
            SkillsUsed = skillsUsed;

        MarkAsUpdated();
        AddDomainEvent(new ExperienceUpdatedEvent(Id));
    }

    public void AddSkill(Guid skillId)
    {
        if (!SkillsUsed.Contains(skillId))
            SkillsUsed.Add(skillId);
        MarkAsUpdated();
    }

    public void RemoveSkill(Guid skillId)
    {
        SkillsUsed.Remove(skillId);
        MarkAsUpdated();
    }

    /// <summary>
    /// Retorna a duração formatada da experiência
    /// </summary>
    public string GetFormattedDuration()
    {
        var dateRange = new ValueObjects.DateRange(StartDate, EndDate);
        return dateRange.DurationFormatted;
    }
}
