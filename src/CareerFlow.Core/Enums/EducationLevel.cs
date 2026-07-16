namespace CareerFlow.Core.Enums;

/// <summary>
/// Níveis de formação acadêmica.
/// </summary>
public enum EducationLevel
{
    /// <summary>Ensino Fundamental</summary>
    Elementary = 0,

    /// <summary>Ensino Médio</summary>
    HighSchool = 1,

    /// <summary>Ensino Técnico</summary>
    Technical = 2,

    /// <summary>Graduação (Bacharelado, Licenciatura)</summary>
    Graduation = 3,

    /// <summary>Pós-graduação Lato Sensu (Especialização, MBA)</summary>
    PostGraduation = 4,

    /// <summary>Especialização</summary>
    Specialization = 5,

    /// <summary>Mestrado (Stricto Sensu)</summary>
    Master = 6,

    /// <summary>Doutorado</summary>
    Doctorate = 7,

    /// <summary>Outros</summary>
    Other = 8
}

public static class EducationLevelExtensions
{
    private static readonly Dictionary<EducationLevel, string> DisplayNames = new()
    {
        { EducationLevel.Elementary, "Ensino Fundamental" },
        { EducationLevel.HighSchool, "Ensino Médio" },
        { EducationLevel.Technical, "Ensino Técnico" },
        { EducationLevel.Graduation, "Graduação" },
        { EducationLevel.PostGraduation, "Pós-graduação" },
        { EducationLevel.Specialization, "Especialização" },
        { EducationLevel.Master, "Mestrado" },
        { EducationLevel.Doctorate, "Doutorado" },
        { EducationLevel.Other, "Outros" }
    };

    public static string GetDisplayName(this EducationLevel level)
    {
        return DisplayNames.TryGetValue(level, out var name) ? name : level.ToString();
    }
}
