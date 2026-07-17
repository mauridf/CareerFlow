namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Serviço para geração de currículos em PDF.
/// </summary>
public interface IPdfGeneratorService
{
    /// <summary>
    /// Gera um PDF padrão do currículo
    /// </summary>
    /// <param name="resumeData">Dados completos do currículo</param>
    /// <returns>Array de bytes do PDF</returns>
    Task<byte[]> GenerateResumePdfAsync(ResumeData resumeData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera um PDF otimizado para ATS
    /// </summary>
    /// <param name="resumeData">Dados completos do currículo</param>
    /// <returns>Array de bytes do PDF ATS</returns>
    Task<byte[]> GenerateAtsResumePdfAsync(ResumeData resumeData, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO com todos os dados necessários para gerar o currículo
/// </summary>
public class ResumeData
{
    // Dados Pessoais
    public string PersonName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ProfessionalSummary { get; set; }
    public string? PhotoUrl { get; set; }
    public string? CurrentPosition { get; set; }
    public string? CurrentCompany { get; set; }

    // Listas
    public List<ResumeExperienceData> Experiences { get; set; } = new();
    public List<ResumeEducationData> Educations { get; set; } = new();
    public List<ResumeSkillData> Skills { get; set; } = new();
    public List<ResumeLanguageData> Languages { get; set; } = new();
    public List<ResumeCertificateData> Certificates { get; set; } = new();
    public List<ResumeSocialNetworkData> SocialNetworks { get; set; } = new();
}

public class ResumeExperienceData
{
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? EmploymentType { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? DurationFormatted { get; set; }
}

public class ResumeEducationData
{
    public string Institution { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}

public class ResumeSkillData
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class ResumeLanguageData
{
    public string LanguageName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsNative { get; set; }
}

public class ResumeCertificateData
{
    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? CredentialUrl { get; set; }
}

public class ResumeSocialNetworkData
{
    public string NetworkType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
