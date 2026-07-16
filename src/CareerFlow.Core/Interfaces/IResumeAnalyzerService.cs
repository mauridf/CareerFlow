namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Serviço para análise de currículo (compatibilidade ATS, sugestões).
/// </summary>
public interface IResumeAnalyzerService
{
    /// <summary>
    /// Analisa o currículo e retorna score ATS
    /// </summary>
    /// <param name="personId">ID do perfil</param>
    /// <returns>Resultado da análise ATS</returns>
    Task<AtsAnalysisResult> AnalyzeAtsCompatibilityAsync(Guid personId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera sugestões de melhoria para o currículo
    /// </summary>
    /// <param name="personId">ID do perfil</param>
    /// <returns>Lista de sugestões</returns>
    Task<IReadOnlyList<ResumeSuggestionResult>> GenerateSuggestionsAsync(Guid personId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado da análise ATS
/// </summary>
public class AtsAnalysisResult
{
    public int Score { get; set; }
    public int Compatibility { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public List<string> DetectedKeywords { get; set; } = new();
    public List<string> MissingKeywords { get; set; } = new();
}

/// <summary>
/// Sugestão de melhoria do currículo
/// </summary>
public class ResumeSuggestionResult
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "medium";
}
