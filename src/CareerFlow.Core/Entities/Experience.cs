using CareerFlow.Core.Events;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa uma experiência profissional.
/// </summary>
public class Experience : AggregateRoot<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    /// <summary>Nome da empresa</summary>
    public string CompanyName { get; private set; } = string.Empty;

    /// <summary>Cargo exercido</summary>
    public string Position { get; private set; } = string.Empty;

    /// <summary>Data de início</summary>
    public DateTime StartDate { get; private set; }

    /// <summary>Data de término (null = atual)</summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>Trabalho atual</summary>
    public bool IsCurrent { get; private set; }

    /// <summary>Descrição das atividades</summary>
    public string? Description { get; private set; }

    /// <summary>Habilidades utilizadas (IDs)</summary>
    public List<Guid> SkillsUsed { get; private set; } = new();

    /// <summary>Cidade</summary>
    public string? City { get; private set; }

    /// <summary>Estado (UF)</summary>
    public string? State { get; private set; }

    /// <summary>País</summary>
    public string Country { get; private set; } = "Brasil";

    /// <summary>Tipo de emprego</summary>
    public string? EmploymentType { get; private set; }

    /// <summary>Ordem de exibição</summary>
    public int DisplayOrder { get; private set; }

    private Experience() { }

    /// <summary>
    /// Cria uma nova experiência profissional
    /// </summary>
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
        string? employmentType = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Nome da empresa é obrigatório", nameof(companyName));

        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Cargo é obrigatório", nameof(position));

        if (endDate.HasValue && startDate > endDate.Value)
            throw new ArgumentException("Data de início deve ser anterior à data de término");

        if (!string.IsNullOrWhiteSpace(description) && description.Length < 50)
            throw new ArgumentException("Descrição deve ter no mínimo 50 caracteres");

        var experience = new Experience
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            CompanyName = companyName.Trim(),
            Position = position.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            IsCurrent = !endDate.HasValue,
            Description = description?.Trim(),
            SkillsUsed = skillsUsed ?? new List<Guid>(),
            City = city?.Trim(),
            State = state?.Trim()?.ToUpper(),
            Country = country?.Trim() ?? "Brasil",
            EmploymentType = employmentType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        experience.AddDomainEvent(new ExperienceCreatedEvent(experience.Id, personId));

        return experience;
    }

    /// <summary>
    /// Atualiza a experiência profissional
    /// </summary>
    public void Update(
        string companyName,
        string position,
        DateTime startDate,
        DateTime? endDate,
        string? description,
        string? city = null,
        string? state = null,
        string? employmentType = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Nome da empresa é obrigatório", nameof(companyName));

        CompanyName = companyName.Trim();
        Position = position.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsCurrent = !endDate.HasValue;
        Description = description?.Trim();
        City = city?.Trim();
        State = state?.Trim()?.ToUpper();
        EmploymentType = employmentType;

        MarkAsUpdated();
    }

    /// <summary>
    /// Adiciona uma habilidade utilizada
    /// </summary>
    public void AddSkill(Guid skillId)
    {
        if (!SkillsUsed.Contains(skillId))
            SkillsUsed.Add(skillId);
        MarkAsUpdated();
    }

    /// <summary>
    /// Remove uma habilidade utilizada
    /// </summary>
    public void RemoveSkill(Guid skillId)
    {
        SkillsUsed.Remove(skillId);
        MarkAsUpdated();
    }
}
