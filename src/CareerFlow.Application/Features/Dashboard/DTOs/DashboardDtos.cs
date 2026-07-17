namespace CareerFlow.Application.Features.Dashboard.DTOs;

public class DashboardStatsResponse
{
    public int TotalViews { get; set; }
    public int UniqueViews { get; set; }
    public int PdfDownloads { get; set; }
    public int SharesCount { get; set; }
    public int? AtsScore { get; set; }
    public int CompletionPercentage { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public bool IsPublished { get; set; }
    public int SkillsCount { get; set; }
    public int ExperiencesCount { get; set; }
    public int EducationCount { get; set; }
    public int CertificatesCount { get; set; }
    public int LanguagesCount { get; set; }
}

public class ResumeInsightsResponse
{
    public string? ProfessionalSummary { get; set; }
    public int CompletionPercentage { get; set; }
    public bool CanGenerateResume { get; set; }
    public int? AtsScore { get; set; }
    public List<string> Strengths { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class ActivityResponse
{
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ViewsChartResponse
{
    public List<ViewsDataPoint> DataPoints { get; set; } = new();
}

public class ViewsDataPoint
{
    public string Period { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Downloads { get; set; }
}

public class SkillsGapResponse
{
    public List<string> PresentSkills { get; set; } = new();
    public List<string> RecommendedSkills { get; set; } = new();
    public string? PrimaryCategory { get; set; }
}
