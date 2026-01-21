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

    public IFormFile? CertificateFile { get; set; }
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