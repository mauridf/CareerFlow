namespace CareerFlow.Core.Enums;

/// <summary>
/// Status da formação acadêmica.
/// </summary>
public enum EducationStatus
{
    /// <summary>Em andamento</summary>
    InProgress = 0,

    /// <summary>Concluído</summary>
    Completed = 1,

    /// <summary>Pausado/Trancado</summary>
    Paused = 2,

    /// <summary>Abandonado</summary>
    DroppedOut = 3
}

public static class EducationStatusExtensions
{
    private static readonly Dictionary<EducationStatus, string> DisplayNames = new()
    {
        { EducationStatus.InProgress, "Em andamento" },
        { EducationStatus.Completed, "Concluído" },
        { EducationStatus.Paused, "Pausado" },
        { EducationStatus.DroppedOut, "Trancado" }
    };

    public static string GetDisplayName(this EducationStatus status)
    {
        return DisplayNames.TryGetValue(status, out var name) ? name : status.ToString();
    }

    public static bool IsFinished(this EducationStatus status)
    {
        return status == EducationStatus.Completed || status == EducationStatus.DroppedOut;
    }
}
