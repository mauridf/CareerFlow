using System.ComponentModel;

namespace CareerFlow.Core.Enums;

/// <summary>
/// Categorias de habilidades técnicas e comportamentais.
/// </summary>
public enum SkillCategory
{
    /// <summary>Backend (APIs, servidores, lógica de negócio)</summary>
    [Description("Backend")]
    Backend = 0,

    /// <summary>Frontend (interfaces, UX/UI)</summary>
    [Description("Frontend")]
    Frontend = 1,

    /// <summary>Banco de Dados (SQL, NoSQL, modelagem)</summary>
    [Description("Banco de Dados")]
    Database = 2,

    /// <summary>Cloud e DevOps (AWS, Azure, Docker, CI/CD)</summary>
    [Description("Cloud & DevOps")]
    CloudDevOps = 3,

    /// <summary>Arquitetura e Padrões de Projeto</summary>
    [Description("Arquitetura & Padrões")]
    ArchitecturePatterns = 4,

    /// <summary>Ferramentas (IDEs, versionamento, produtividade)</summary>
    [Description("Ferramentas")]
    Tools = 5,

    /// <summary>Soft Skills (comunicação, liderança, trabalho em equipe)</summary>
    [Description("Soft Skills")]
    SoftSkills = 6,

    /// <summary>Outras habilidades não categorizadas</summary>
    [Description("Outras Habilidades")]
    Other = 7
}

/// <summary>
/// Extensões para o enum SkillCategory
/// </summary>
public static class SkillCategoryExtensions
{
    private static readonly Dictionary<SkillCategory, string> DisplayNames = new()
    {
        { SkillCategory.Backend, "Backend" },
        { SkillCategory.Frontend, "Frontend" },
        { SkillCategory.Database, "Banco de Dados" },
        { SkillCategory.CloudDevOps, "Cloud & DevOps" },
        { SkillCategory.ArchitecturePatterns, "Arquitetura & Padrões" },
        { SkillCategory.Tools, "Ferramentas" },
        { SkillCategory.SoftSkills, "Soft Skills" },
        { SkillCategory.Other, "Outras Habilidades" }
    };

    private static readonly Dictionary<SkillCategory, string> Icons = new()
    {
        { SkillCategory.Backend, "⚙️" },
        { SkillCategory.Frontend, "🎨" },
        { SkillCategory.Database, "🗄️" },
        { SkillCategory.CloudDevOps, "☁️" },
        { SkillCategory.ArchitecturePatterns, "🏗️" },
        { SkillCategory.Tools, "🔧" },
        { SkillCategory.SoftSkills, "🤝" },
        { SkillCategory.Other, "📦" }
    };

    /// <summary>
    /// Retorna o nome amigável da categoria
    /// </summary>
    public static string GetDisplayName(this SkillCategory category)
    {
        return DisplayNames.TryGetValue(category, out var name) ? name : category.ToString();
    }

    /// <summary>
    /// Retorna o ícone representativo da categoria
    /// </summary>
    public static string GetIcon(this SkillCategory category)
    {
        return Icons.TryGetValue(category, out var icon) ? icon : "📦";
    }

    /// <summary>
    /// Retorna todas as categorias disponíveis
    /// </summary>
    public static IReadOnlyList<SkillCategory> GetAll()
    {
        return Enum.GetValues<SkillCategory>().ToList().AsReadOnly();
    }

    /// <summary>
    /// Retorna categorias técnicas (exclui soft skills e outros)
    /// </summary>
    public static IReadOnlyList<SkillCategory> GetTechnicalCategories()
    {
        return new List<SkillCategory>
        {
            SkillCategory.Backend,
            SkillCategory.Frontend,
            SkillCategory.Database,
            SkillCategory.CloudDevOps,
            SkillCategory.ArchitecturePatterns,
            SkillCategory.Tools
        }.AsReadOnly();
    }
}
