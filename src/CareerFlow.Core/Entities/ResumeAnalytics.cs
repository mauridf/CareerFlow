using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Analytics agregados do currículo.
/// Atualizado periodicamente pelo Scheduler.
/// </summary>
public class ResumeAnalytics : Entity<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    // Métricas de visualização
    public int TotalViews { get; private set; }
    public int UniqueViews { get; private set; }
    public int PdfDownloads { get; private set; }
    public int SharesCount { get; private set; }

    // Análise ATS
    public int? AtsScore { get; private set; }
    public int? AtsCompatibility { get; private set; }
    public string? AtsIssues { get; private set; }
    public string? AtsSuggestions { get; private set; }

    // Engajamento
    public int? AverageViewDurationSeconds { get; private set; }
    public DateTime? LastViewedAt { get; private set; }
    public string? DetectedKeywords { get; private set; }
    public string? MissingKeywords { get; private set; }
    public DateTime? AnalyzedAt { get; private set; }

    // ============================================
    // Status do Currículo (usando ResumeStatus enum)
    // ============================================

    /// <summary>Status atual do currículo</summary>
    public ResumeStatus Status { get; private set; } = ResumeStatus.Draft;

    /// <summary>Data da última publicação</summary>
    public DateTime? PublishedAt { get; private set; }

    private ResumeAnalytics() { }

    public static ResumeAnalytics Create(Guid personId)
    {
        return new ResumeAnalytics
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Status = ResumeStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ============================================
    // Métodos de Comportamento
    // ============================================

    public void IncrementView()
    {
        TotalViews++;
        LastViewedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void IncrementUniqueView()
    {
        UniqueViews++;
        MarkAsUpdated();
    }

    public void IncrementPdfDownload()
    {
        PdfDownloads++;
        MarkAsUpdated();
    }

    public void IncrementShare()
    {
        SharesCount++;
        MarkAsUpdated();
    }

    public void UpdateAtsAnalysis(int score, int compatibility, string? issues, string? suggestions)
    {
        AtsScore = score;
        AtsCompatibility = compatibility;
        AtsIssues = issues;
        AtsSuggestions = suggestions;
        AnalyzedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void UpdateKeywords(string? detected, string? missing)
    {
        DetectedKeywords = detected;
        MissingKeywords = missing;
        MarkAsUpdated();
    }

    public void Publish()
    {
        if (Status == ResumeStatus.Published)
            return;

        Status = ResumeStatus.Published;
        PublishedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Unpublish()
    {
        if (Status != ResumeStatus.Published)
            return;

        Status = ResumeStatus.Draft;
        MarkAsUpdated();
    }

    public void Archive()
    {
        Status = ResumeStatus.Archived;
        MarkAsUpdated();
    }

    public bool IsPubliclyVisible()
    {
        return Status == ResumeStatus.Published;
    }
}
