namespace CareerFlow.Core.Enums;

/// <summary>
/// Níveis de proficiência em habilidades.
/// </summary>
public enum ProficiencyLevel
{
    /// <summary>Conhecimento básico, necessita supervisão</summary>
    Basic = 0,

    /// <summary>Conhecimento intermediário, trabalha de forma independente</summary>
    Intermediate = 1,

    /// <summary>Conhecimento avançado, capaz de mentorar</summary>
    Advanced = 2,

    /// <summary>Especialista, referência na área</summary>
    Expert = 3
}

/// <summary>
/// Extensões para o enum ProficiencyLevel
/// </summary>
public static class ProficiencyLevelExtensions
{
    private static readonly Dictionary<ProficiencyLevel, string> DisplayNames = new()
    {
        { ProficiencyLevel.Basic, "Básico" },
        { ProficiencyLevel.Intermediate, "Intermediário" },
        { ProficiencyLevel.Advanced, "Avançado" },
        { ProficiencyLevel.Expert, "Especialista" }
    };

    private static readonly Dictionary<ProficiencyLevel, int> Scores = new()
    {
        { ProficiencyLevel.Basic, 25 },
        { ProficiencyLevel.Intermediate, 50 },
        { ProficiencyLevel.Advanced, 75 },
        { ProficiencyLevel.Expert, 100 }
    };

    /// <summary>
    /// Retorna o nome amigável do nível
    /// </summary>
    public static string GetDisplayName(this ProficiencyLevel level)
    {
        return DisplayNames.TryGetValue(level, out var name) ? name : level.ToString();
    }

    /// <summary>
    /// Retorna o score numérico (0-100)
    /// </summary>
    public static int GetScore(this ProficiencyLevel level)
    {
        return Scores.TryGetValue(level, out var score) ? score : 0;
    }
}
