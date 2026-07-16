namespace CareerFlow.Core.Enums;

/// <summary>
/// Níveis de proficiência em idiomas (CEFR - Common European Framework of Reference).
/// </summary>
public enum LanguageLevel
{
    /// <summary>Iniciante (A1)</summary>
    A1 = 0,

    /// <summary>Básico (A2)</summary>
    A2 = 1,

    /// <summary>Intermediário (B1)</summary>
    B1 = 2,

    /// <summary>Intermediário superior (B2)</summary>
    B2 = 3,

    /// <summary>Avançado (C1)</summary>
    C1 = 4,

    /// <summary>Proficiente (C2)</summary>
    C2 = 5,

    /// <summary>Nativo</summary>
    Native = 6
}

public static class LanguageLevelExtensions
{
    private static readonly Dictionary<LanguageLevel, string> DisplayNames = new()
    {
        { LanguageLevel.A1, "A1 - Iniciante" },
        { LanguageLevel.A2, "A2 - Básico" },
        { LanguageLevel.B1, "B1 - Intermediário" },
        { LanguageLevel.B2, "B2 - Intermediário Superior" },
        { LanguageLevel.C1, "C1 - Avançado" },
        { LanguageLevel.C2, "C2 - Proficiente" },
        { LanguageLevel.Native, "Nativo" }
    };

    private static readonly Dictionary<LanguageLevel, int> Scores = new()
    {
        { LanguageLevel.A1, 10 },
        { LanguageLevel.A2, 25 },
        { LanguageLevel.B1, 45 },
        { LanguageLevel.B2, 65 },
        { LanguageLevel.C1, 85 },
        { LanguageLevel.C2, 95 },
        { LanguageLevel.Native, 100 }
    };

    /// <summary>
    /// Retorna o nome descritivo do nível
    /// </summary>
    public static string GetDisplayName(this LanguageLevel level)
    {
        return DisplayNames.TryGetValue(level, out var name) ? name : level.ToString();
    }

    /// <summary>
    /// Retorna o nome curto (ex: "B2", "C1", "Nativo")
    /// </summary>
    public static string GetShortName(this LanguageLevel level)
    {
        return level == LanguageLevel.Native ? "Nativo" : level.ToString();
    }

    /// <summary>
    /// Retorna o score numérico (0-100)
    /// </summary>
    public static int GetScore(this LanguageLevel level)
    {
        return Scores.TryGetValue(level, out var score) ? score : 0;
    }

    /// <summary>
    /// Retorna o nível CEFR (A1-C2) ou "Native"
    /// </summary>
    public static string GetCefrLevel(this LanguageLevel level)
    {
        return level == LanguageLevel.Native ? "Native" : level.ToString();
    }
}
