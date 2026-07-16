namespace CareerFlow.Core.Enums;

/// <summary>
/// Status de publicação do currículo.
/// </summary>
public enum ResumeStatus
{
    /// <summary>Rascunho (não publicado)</summary>
    Draft = 0,

    /// <summary>Publicado (visível publicamente)</summary>
    Published = 1,

    /// <summary>Arquivado (não visível, mas preservado)</summary>
    Archived = 2
}

public static class ResumeStatusExtensions
{
    private static readonly Dictionary<ResumeStatus, string> DisplayNames = new()
    {
        { ResumeStatus.Draft, "Rascunho" },
        { ResumeStatus.Published, "Publicado" },
        { ResumeStatus.Archived, "Arquivado" }
    };

    public static string GetDisplayName(this ResumeStatus status)
    {
        return DisplayNames.TryGetValue(status, out var name) ? name : status.ToString();
    }

    /// <summary>
    /// Verifica se o currículo está visível publicamente
    /// </summary>
    public static bool IsPubliclyVisible(this ResumeStatus status)
    {
        return status == ResumeStatus.Published;
    }
}
