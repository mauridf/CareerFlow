namespace CareerFlow.Core.Entities;

/// <summary>
/// Analytics agregados do currículo.
/// </summary>
public class ResumeAnalytics : Entity<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public int TotalViews { get; private set; }
    public int UniqueViews { get; private set; }
    public int PdfDownloads { get; private set; }
    public int SharesCount { get; private set; }
    public int? AtsScore { get; private set; }
    public int? AtsCompatibility { get; private set; }
    public string? AtsIssues { get; private set; }
    public string? AtsSuggestions { get; private set; }
    public int? AverageViewDurationSeconds { get; private set; }
    public DateTime? LastViewedAt { get; private set; }
    public string? DetectedKeywords { get; private set; }
    public string? MissingKeywords { get; private set; }
    public DateTime? AnalyzedAt { get; private set; }

    private ResumeAnalytics() { }

    public static ResumeAnalytics Create(Guid personId)
    {
        return new ResumeAnalytics
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void IncrementView()
    {
        TotalViews++;
        LastViewedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void IncrementPdfDownload()
    {
        PdfDownloads++;
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
}
