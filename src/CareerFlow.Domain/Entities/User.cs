using CareerFlow.Domain.Common;

namespace CareerFlow.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PhotoPath { get; set; }

    // Navigation Properties
    public ProfessionalSummary? ProfessionalSummary { get; set; }
    public ICollection<SocialMedia> SocialMedias { get; set; } = new List<SocialMedia>();
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<ProfessionalExperience> ProfessionalExperiences { get; set; } = new List<ProfessionalExperience>();
    public ICollection<AcademicBackground> AcademicBackgrounds { get; set; } = new List<AcademicBackground>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public ICollection<Language> Languages { get; set; } = new List<Language>();
}