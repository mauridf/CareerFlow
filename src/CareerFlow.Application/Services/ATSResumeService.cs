using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Services;

public class ATSResumeService : ServiceBase, IATSResumeService
{
    private readonly IProfileService _profileService;
    private readonly ISkillService _skillService;
    private readonly IExperienceService _experienceService;

    public ATSResumeService(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<ATSResumeService> logger,
        IProfileService profileService,
        ISkillService skillService,
        IExperienceService experienceService)
        : base(context, mapper, logger)
    {
        _profileService = profileService;
        _skillService = skillService;
        _experienceService = experienceService;
    }

    public async Task<ATSResumeDto> GenerateATSResumeAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException($"Usuário com ID {userId} não encontrado.");

        var resumeData = await _profileService.GetResumeDataAsync(userId);

        var atsResume = new ATSResumeDto
        {
            FullName = user.Name,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            Location = $"{user.City ?? string.Empty}{(user.City != null && user.State != null ? ", " : "")}{user.State ?? string.Empty}",
            ProfessionalSummary = resumeData.Summary?.Summary ?? string.Empty,
            Skills = MapToATSSkills(resumeData.Skills),
            Experiences = MapToATSExperiences(resumeData.Experiences),
            Education = MapToATSEducation(resumeData.Academics),
            Certifications = MapToATSCertifications(resumeData.Certificates),
            Languages = MapToATSLanguages(resumeData.Languages)
        };

        return atsResume;
    }

    public async Task<byte[]> GenerateATSResumePdfAsync(Guid userId)
    {
        var atsResume = await GenerateATSResumeAsync(userId);

        // Para simplificar, vamos gerar um texto formatado
        // Em produção, use uma biblioteca como QuestPDF, iTextSharp, ou PuppeteerSharp
        var resumeText = FormatResumeAsText(atsResume);

        return System.Text.Encoding.UTF8.GetBytes(resumeText);
    }

    public async Task<byte[]> GenerateATSResumeJsonAsync(Guid userId)
    {
        var atsResume = await GenerateATSResumeAsync(userId);
        var json = System.Text.Json.JsonSerializer.Serialize(atsResume, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public async Task<string> GetATSResumeTextAsync(Guid userId)
    {
        var atsResume = await GenerateATSResumeAsync(userId);
        return FormatResumeAsText(atsResume);
    }

    public async Task<List<string>> GetATSKeywordsAsync(Guid userId)
    {
        var keywords = new List<string>();
        var resumeData = await _profileService.GetResumeDataAsync(userId);

        // Adicionar nome das skills
        keywords.AddRange(resumeData.Skills.Select(s => s.Name));

        // Adicionar cargos/posições
        keywords.AddRange(resumeData.Experiences.Select(e => e.Position));

        // Adicionar empresas
        keywords.AddRange(resumeData.Experiences.Select(e => e.Company));

        // Adicionar áreas de formação
        keywords.AddRange(resumeData.Academics.Select(a => a.CourseName));

        // Adicionar certificações
        keywords.AddRange(resumeData.Certificates.Select(c => c.Name));

        // Remover duplicados e vazios
        return keywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();
    }

    public async Task<int> CalculateATSScoreAsync(Guid userId)
    {
        var score = 0;
        var resumeData = await _profileService.GetResumeDataAsync(userId);

        // Critérios de pontuação para ATS
        if (!string.IsNullOrEmpty(resumeData.Summary?.Summary)) score += 10;
        if (resumeData.Skills.Any()) score += 20;
        if (resumeData.Experiences.Any()) score += 30;
        if (resumeData.Academics.Any()) score += 15;
        if (resumeData.Certificates.Any()) score += 15;
        if (resumeData.Languages.Any()) score += 10;

        // Pontuação extra para experiências atuais
        var currentExperiences = resumeData.Experiences.Count(e => e.IsCurrent);
        score += currentExperiences * 5;

        // Pontuação extra para habilidades avançadas
        var advancedSkills = resumeData.Skills.Count(s => s.Level == "AVANÇADO");
        score += advancedSkills * 2;

        return Math.Min(score, 100); // Máximo 100 pontos
    }

    private List<ATSSkillDto> MapToATSSkills(List<SkillDto> skills)
    {
        return skills.Select(s => new ATSSkillDto
        {
            Name = s.Name,
            Type = s.Type,
            Level = s.Level
        }).ToList();
    }

    private List<ATSExperienceDto> MapToATSExperiences(List<ProfessionalExperienceDto> experiences)
    {
        return experiences.Select(e => new ATSExperienceDto
        {
            Company = e.Company,
            Position = e.Position,
            Period = FormatPeriod(e.StartDate, e.EndDate),
            Responsibilities = e.Responsibilities.Split('\n').Where(r => !string.IsNullOrWhiteSpace(r)).ToList(),
            SkillsUsed = e.Skills.Select(s => s.Name).ToList()
        }).ToList();
    }

    private List<ATSEducationDto> MapToATSEducation(List<AcademicBackgroundDto> academics)
    {
        return academics.Select(a => new ATSEducationDto
        {
            Institution = a.Institution,
            Degree = $"{a.CourseName} - {a.Level}",
            Period = FormatPeriod(a.StartDate, a.EndDate)
        }).ToList();
    }

    private List<ATSCertificationDto> MapToATSCertifications(List<CertificateDto> certificates)
    {
        return certificates.Select(c => new ATSCertificationDto
        {
            Name = c.Name,
            Issuer = "Certificado", // Em produção, extrair o emissor do nome ou ter campo separado
            Validity = c.EndDate.HasValue ? c.EndDate.Value.ToString("MM/yyyy") : "Válido indefinidamente"
        }).ToList();
    }

    private List<ATSLanguageDto> MapToATSLanguages(List<LanguageDto> languages)
    {
        return languages.Select(l => new ATSLanguageDto
        {
            Language = l.Name,
            Proficiency = l.Level
        }).ToList();
    }

    private string FormatPeriod(DateTime startDate, DateTime? endDate)
    {
        var start = startDate.ToString("MM/yyyy");
        var end = endDate?.ToString("MM/yyyy") ?? "Atual";
        return $"{start} - {end}";
    }

    private string FormatResumeAsText(ATSResumeDto resume)
    {
        var text = new System.Text.StringBuilder();

        text.AppendLine("=".PadRight(80, '='));
        text.AppendLine($"CURRÍCULO ATS - {resume.FullName.ToUpper()}");
        text.AppendLine("=".PadRight(80, '='));
        text.AppendLine();

        // Informações de contato
        text.AppendLine("INFORMAÇÕES DE CONTATO");
        text.AppendLine("-".PadRight(40, '-'));
        text.AppendLine($"Nome: {resume.FullName}");
        text.AppendLine($"Email: {resume.Email}");
        if (!string.IsNullOrEmpty(resume.Phone))
            text.AppendLine($"Telefone: {resume.Phone}");
        if (!string.IsNullOrEmpty(resume.Location))
            text.AppendLine($"Localização: {resume.Location}");
        text.AppendLine();

        // Resumo profissional
        if (!string.IsNullOrEmpty(resume.ProfessionalSummary))
        {
            text.AppendLine("RESUMO PROFISSIONAL");
            text.AppendLine("-".PadRight(40, '-'));
            text.AppendLine(resume.ProfessionalSummary);
            text.AppendLine();
        }

        // Experiência profissional
        if (resume.Experiences.Any())
        {
            text.AppendLine("EXPERIÊNCIA PROFISSIONAL");
            text.AppendLine("-".PadRight(40, '-'));
            foreach (var exp in resume.Experiences)
            {
                text.AppendLine($"{exp.Position} | {exp.Company}");
                text.AppendLine($"Período: {exp.Period}");
                if (exp.Responsibilities.Any())
                {
                    text.AppendLine("Responsabilidades:");
                    foreach (var resp in exp.Responsibilities)
                    {
                        text.AppendLine($"  • {resp}");
                    }
                }
                if (exp.SkillsUsed.Any())
                {
                    text.AppendLine($"Habilidades utilizadas: {string.Join(", ", exp.SkillsUsed)}");
                }
                text.AppendLine();
            }
        }

        // Habilidades
        if (resume.Skills.Any())
        {
            text.AppendLine("HABILIDADES");
            text.AppendLine("-".PadRight(40, '-'));
            var skillsByType = resume.Skills.GroupBy(s => s.Type);
            foreach (var group in skillsByType)
            {
                text.AppendLine($"{group.Key}:");
                foreach (var skill in group)
                {
                    text.AppendLine($"  • {skill.Name} ({skill.Level})");
                }
            }
            text.AppendLine();
        }

        // Formação acadêmica
        if (resume.Education.Any())
        {
            text.AppendLine("FORMAÇÃO ACADÊMICA");
            text.AppendLine("-".PadRight(40, '-'));
            foreach (var edu in resume.Education)
            {
                text.AppendLine($"{edu.Degree}");
                text.AppendLine($"{edu.Institution} | {edu.Period}");
                text.AppendLine();
            }
        }

        // Certificações
        if (resume.Certifications.Any())
        {
            text.AppendLine("CERTIFICAÇÕES");
            text.AppendLine("-".PadRight(40, '-'));
            foreach (var cert in resume.Certifications)
            {
                text.AppendLine($"{cert.Name}");
                text.AppendLine($"Emitido por: {cert.Issuer} | Validade: {cert.Validity}");
                text.AppendLine();
            }
        }

        // Idiomas
        if (resume.Languages.Any())
        {
            text.AppendLine("IDIOMAS");
            text.AppendLine("-".PadRight(40, '-'));
            foreach (var lang in resume.Languages)
            {
                text.AppendLine($"{lang.Language}: {lang.Proficiency}");
            }
            text.AppendLine();
        }

        text.AppendLine("=".PadRight(80, '='));
        text.AppendLine("Currículo gerado pelo CareerFlow - Otimizado para sistemas ATS");
        text.AppendLine($"Data de geração: {DateTime.Now:dd/MM/yyyy HH:mm}");

        return text.ToString();
    }
}