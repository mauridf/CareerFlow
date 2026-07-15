using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa uma habilidade do usuário.
/// </summary>
public class Skill : Entity<Guid>
{
    /// <summary>ID do perfil associado</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Perfil associado</summary>
    public Person? Person { get; private set; }

    /// <summary>Nome da habilidade</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Categoria da habilidade</summary>
    public SkillCategory Category { get; private set; }

    /// <summary>Nível de proficiência</summary>
    public ProficiencyLevel ProficiencyLevel { get; private set; } = ProficiencyLevel.Basic;

    /// <summary>Habilidade principal (destaque)</summary>
    public bool IsPrimary { get; private set; }

    /// <summary>Ordem de exibição</summary>
    public int DisplayOrder { get; private set; }

    private Skill() { }

    /// <summary>
    /// Cria uma nova habilidade
    /// </summary>
    public static Skill Create(
        Guid personId,
        string name,
        SkillCategory category,
        ProficiencyLevel level,
        bool isPrimary = false,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da habilidade é obrigatório", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Nome da habilidade deve ter no máximo 100 caracteres", nameof(name));

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

    /// <summary>
    /// Atualiza a habilidade
    /// </summary>
    public void Update(string name, SkillCategory category, ProficiencyLevel level, bool isPrimary, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da habilidade é obrigatório", nameof(name));

        Name = name.Trim();
        Category = category;
        ProficiencyLevel = level;
        IsPrimary = isPrimary;
        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }
}
