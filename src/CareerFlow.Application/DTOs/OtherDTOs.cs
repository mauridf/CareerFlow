using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CareerFlow.Application.DTOs;

// Certificate DTOs
public class CertificateDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsValid { get; set; }
    public string? CertificatePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCertificateDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    // Alterado de IFormFile para string
    public string? CertificateFile { get; set; }
}

public class UpdateCertificateDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    // Alterado de IFormFile para string
    public string? CertificateFile { get; set; }
}

// Language DTOs
public class LanguageDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLanguageDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Level { get; set; } = string.Empty;
}

public class UpdateLanguageDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    public string? Level { get; set; }
}

// Social Media DTOs
public class SocialMediaDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSocialMediaDto
{
    [Required]
    [StringLength(100)]
    public string Platform { get; set; } = string.Empty;

    [Required]
    [Url]
    [StringLength(500)]
    public string Url { get; set; } = string.Empty;
}

public class UpdateSocialMediaDto
{
    [StringLength(100)]
    public string? Platform { get; set; }

    [Url]
    [StringLength(500)]
    public string? Url { get; set; }
}

// Dashboard DTOs
public class DashboardStatsDto
{
    public int TotalSkills { get; set; }
    public int TotalExperiences { get; set; }
    public int TotalCertificates { get; set; }
    public int TotalLanguages { get; set; }
    public int ProfileCompleteness { get; set; }
    public List<SkillDistributionDto> SkillDistribution { get; set; } = new();
    public List<UpcomingExpirationDto> UpcomingExpirations { get; set; } = new();
}

public class SkillDistributionDto
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; }
}

public class UpcomingExpirationDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "certificate" or "academic"
    public DateTime? ExpirationDate { get; set; }
    public int DaysUntilExpiration { get; set; }
}

// CV/Resume DTOs
public class ResumeDataDto
{
    public UserDto User { get; set; } = null!;
    public ProfessionalSummaryDto? Summary { get; set; }
    public List<SocialMediaDto> SocialMedias { get; set; } = new();
    public List<SkillDto> Skills { get; set; } = new();
    public List<ProfessionalExperienceDto> Experiences { get; set; } = new();
    public List<AcademicBackgroundDto> Academics { get; set; } = new();
    public List<CertificateDto> Certificates { get; set; } = new();
    public List<LanguageDto> Languages { get; set; } = new();
}

// Search and Filter DTOs
public class SearchFilterDto
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

// File Upload DTOs
public class FileUploadDto
{
    public IFormFile File { get; set; } = null!;
}

public class FileUploadResponseDto
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

// ATS (Applicant Tracking System) DTOs
public class ATSResumeDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ProfessionalSummary { get; set; } = string.Empty;
    public List<ATSSkillDto> Skills { get; set; } = new();
    public List<ATSExperienceDto> Experiences { get; set; } = new();
    public List<ATSEducationDto> Education { get; set; } = new();
    public List<ATSCertificationDto> Certifications { get; set; } = new();
    public List<ATSLanguageDto> Languages { get; set; } = new();
}

public class ATSSkillDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public class ATSExperienceDto
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public List<string> Responsibilities { get; set; } = new();
    public List<string> SkillsUsed { get; set; } = new();
}

public class ATSEducationDto
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
}

public class ATSCertificationDto
{
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Validity { get; set; } = string.Empty;
}

public class ATSLanguageDto
{
    public string Language { get; set; } = string.Empty;
    public string Proficiency { get; set; } = string.Empty;
}

// Report and Export DTOs
public class ExportRequestDto
{
    public string Format { get; set; } = "pdf"; // pdf, json, xml, docx
    public bool IncludePersonalInfo { get; set; } = true;
    public bool IncludeSummary { get; set; } = true;
    public bool IncludeSkills { get; set; } = true;
    public bool IncludeExperiences { get; set; } = true;
    public bool IncludeEducation { get; set; } = true;
    public bool IncludeCertifications { get; set; } = true;
    public bool IncludeLanguages { get; set; } = true;
}

public class ExportResponseDto
{
    public string DownloadUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

// Analytics DTOs
public class AnalyticsDto
{
    public int ProfileViews { get; set; }
    public int ResumeDownloads { get; set; }
    public int ProfileCompletions { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, int> SkillDemand { get; set; } = new();
}

// Import DTOs
public class LinkedInImportDto
{
    public string LinkedInUrl { get; set; } = string.Empty;
    public bool ImportExperiences { get; set; } = true;
    public bool ImportEducation { get; set; } = true;
    public bool ImportSkills { get; set; } = true;
    public bool ImportCertifications { get; set; } = true;
}

public class ImportResultDto
{
    public int ImportedItems { get; set; }
    public int SkippedItems { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

// Notification DTOs
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // info, warning, success, error
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
}

// Settings DTOs
public class UserSettingsDto
{
    public bool EmailNotifications { get; set; } = true;
    public bool PublicProfile { get; set; } = false;
    public string TimeZone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string Language { get; set; } = "pt-BR";
    public AtsSettingsDto AtsSettings { get; set; } = new();
}

public class AtsSettingsDto
{
    public bool OptimizeForATS { get; set; } = true;
    public string PreferredTemplate { get; set; } = "modern";
    public bool IncludeKeywords { get; set; } = true;
    public bool ShowSkillLevels { get; set; } = true;
    public bool CompactFormat { get; set; } = false;
}

// Validation DTOs (para validação de formulários)
public class ValidationErrorDto
{
    public string Field { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ValidationErrorDto> Errors { get; set; } = new();
}