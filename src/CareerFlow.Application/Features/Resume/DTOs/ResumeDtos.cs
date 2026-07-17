namespace CareerFlow.Application.Features.Resume.DTOs;

public class ResumeResponse
{
    public PersonInfo Person { get; set; } = new();
    public List<ExperienceInfo> Experiences { get; set; } = new();
    public List<EducationInfo> Educations { get; set; } = new();
    public List<SkillInfo> Skills { get; set; } = new();
    public List<LanguageInfo> Languages { get; set; } = new();
    public List<CertificateInfo> Certificates { get; set; } = new();
    public List<SocialNetworkInfo> SocialNetworks { get; set; } = new();
}

public class PersonInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ProfessionalSummary { get; set; }
    public string? PhotoUrl { get; set; }
    public string? CurrentPosition { get; set; }
    public string? CurrentCompany { get; set; }
    public string? ResumeSlug { get; set; }
}

public class ExperienceInfo
{
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? EmploymentType { get; set; }
    public string DurationFormatted { get; set; } = string.Empty;
}

public class EducationInfo
{
    public string Institution { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class SkillInfo
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int Score { get; set; }
    public bool IsPrimary { get; set; }
}

public class LanguageInfo
{
    public string LanguageName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsNative { get; set; }
}

public class CertificateInfo
{
    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? CredentialUrl { get; set; }
}

public class SocialNetworkInfo
{
    public string NetworkType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class ResumeAnalyticsResponse
{
    public int TotalViews { get; set; }
    public int UniqueViews { get; set; }
    public int PdfDownloads { get; set; }
    public int SharesCount { get; set; }
    public int? AtsScore { get; set; }
    public int? AtsCompatibility { get; set; }
    public string? AtsIssues { get; set; }
    public string? AtsSuggestions { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CompletionPercentage { get; set; }
}

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
}
