using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa uma habilidade do usuário.
/// </summary>
public class Skill : Entity<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public SkillCategory Category { get; private set; } = SkillCategory.Other;
    public ProficiencyLevel ProficiencyLevel { get; private set; } = ProficiencyLevel.Basic;
    public bool IsPrimary { get; private set; }
    public int DisplayOrder { get; private set; }

    private Skill() { }

    public static Skill Create(
        Guid personId,
        string name,
        SkillCategory category,
        ProficiencyLevel level = ProficiencyLevel.Basic,
        bool isPrimary = false,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da habilidade é obrigatório");

        if (name.Length > 100)
            throw new DomainException("Nome da habilidade deve ter no máximo 100 caracteres");

        return new Skill
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Name = name.Trim(),
            Category = category,
            ProficiencyLevel = level,
            IsPrimary = isPrimary,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, SkillCategory category, ProficiencyLevel level, bool isPrimary, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da habilidade é obrigatório");

        if (name.Length > 100)
            throw new DomainException("Nome da habilidade deve ter no máximo 100 caracteres");

        Name = name.Trim();
        Category = category;
        ProficiencyLevel = level;
        IsPrimary = isPrimary;
        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }

    public void TogglePrimary()
    {
        IsPrimary = !IsPrimary;
        MarkAsUpdated();
    }

    /// <summary>
    /// Retorna o score da habilidade (0-100)
    /// </summary>
    public int GetScore()
    {
        return ProficiencyLevel.GetScore();
    }
}
