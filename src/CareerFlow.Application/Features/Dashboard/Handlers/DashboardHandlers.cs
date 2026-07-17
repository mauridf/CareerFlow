using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Dashboard.DTOs;
using CareerFlow.Application.Features.Dashboard.Queries;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Specifications;

namespace CareerFlow.Application.Features.Dashboard.Handlers;

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeViewRepository _viewRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ISkillRepository _skillRepo;
    private readonly IExperienceRepository _experienceRepo;
    private readonly IEducationRepository _educationRepo;
    private readonly ICertificateRepository _certificateRepo;
    private readonly ILanguageRepository _languageRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetDashboardStatsHandler> _logger;

    public GetDashboardStatsHandler(
        IPersonRepository personRepo, IResumeViewRepository viewRepo,
        IResumeAnalyticsRepository analyticsRepo, ISkillRepository skillRepo,
        IExperienceRepository experienceRepo, IEducationRepository educationRepo,
        ICertificateRepository certificateRepo, ILanguageRepository languageRepo,
        ICurrentUserService currentUser, ILogger<GetDashboardStatsHandler> logger)
    {
        _personRepo = personRepo; _viewRepo = viewRepo;
        _analyticsRepo = analyticsRepo; _skillRepo = skillRepo;
        _experienceRepo = experienceRepo; _educationRepo = educationRepo;
        _certificateRepo = certificateRepo; _languageRepo = languageRepo;
        _currentUser = currentUser; _logger = logger;
    }

    public async Task<DashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct);
        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);

        var totalViews = await _viewRepo.CountByPersonIdAsync(personId, ct);
        var uniqueViews = await _viewRepo.CountUniqueByPersonIdAsync(personId, ct);
        var pdfDownloads = await _viewRepo.CountPdfDownloadsAsync(personId, ct);
        var skillsCount = await _skillRepo.CountAsync(s => s.PersonId == personId, ct);
        var expCount = await _experienceRepo.CountByPersonIdAsync(personId, ct);
        var eduCount = await _educationRepo.CountByPersonIdAsync(personId, ct);
        var certCount = await _certificateRepo.CountByPersonIdAsync(personId, ct);
        var langCount = await _languageRepo.CountByPersonIdAsync(personId, ct);

        _logger.LogInformation("📊 Dashboard stats gerados para PersonId={PersonId}", personId);

        return new DashboardStatsResponse
        {
            TotalViews = totalViews,
            UniqueViews = uniqueViews,
            PdfDownloads = pdfDownloads,
            SharesCount = analytics?.SharesCount ?? 0,
            AtsScore = analytics?.AtsScore,
            CompletionPercentage = person?.CalculateCompletionPercentage() ?? 0,
            LastViewedAt = analytics?.LastViewedAt,
            IsPublished = analytics?.IsPubliclyVisible() ?? false,
            SkillsCount = skillsCount,
            ExperiencesCount = expCount,
            EducationCount = eduCount,
            CertificatesCount = certCount,
            LanguagesCount = langCount
        };
    }
}

public class GetResumeInsightsHandler : IRequestHandler<GetResumeInsightsQuery, ResumeInsightsResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ISkillRepository _skillRepo;
    private readonly IExperienceRepository _experienceRepo;
    private readonly IEducationRepository _educationRepo;
    private readonly ICurrentUserService _currentUser;

    public GetResumeInsightsHandler(
        IPersonRepository personRepo, IResumeAnalyticsRepository analyticsRepo,
        ISkillRepository skillRepo, IExperienceRepository experienceRepo,
        IEducationRepository educationRepo, ICurrentUserService currentUser)
    {
        _personRepo = personRepo; _analyticsRepo = analyticsRepo;
        _skillRepo = skillRepo; _experienceRepo = experienceRepo;
        _educationRepo = educationRepo; _currentUser = currentUser;
    }

    public async Task<ResumeInsightsResponse> Handle(GetResumeInsightsQuery request, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct);
        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);
        var skills = await _skillRepo.GetByPersonIdAsync(personId, ct);
        var experiences = await _experienceRepo.GetByPersonIdAsync(personId, ct);
        var educations = await _educationRepo.GetByPersonIdAsync(personId, ct);

        var response = new ResumeInsightsResponse
        {
            ProfessionalSummary = person?.ProfessionalSummary,
            CompletionPercentage = person?.CalculateCompletionPercentage() ?? 0,
            CanGenerateResume = person?.CanGenerateResume() ?? false,
            AtsScore = analytics?.AtsScore
        };

        // Forças (strengths)
        if (experiences.Count >= 3)
            response.Strengths.Add("Experiência profissional sólida (3+ posições)");
        if (skills.Count >= 5)
            response.Strengths.Add("Bom conjunto de habilidades técnicas");
        if (educations.Count >= 1)
            response.Strengths.Add("Formação acadêmica registrada");
        if (skills.Any(s => s.ProficiencyLevel == ProficiencyLevel.Expert))
            response.Strengths.Add("Possui habilidades em nível Especialista");
        if (!string.IsNullOrWhiteSpace(person?.ProfessionalSummary) && person.ProfessionalSummary.Length >= 200)
            response.Strengths.Add("Resumo profissional bem detalhado");

        if (response.Strengths.Count == 0)
            response.Strengths.Add("Perfil iniciado - continue adicionando informações");

        // Melhorias (improvements)
        if (experiences.Count < 2)
            response.Improvements.Add("Adicione mais experiências profissionais (recomendado: 2+)");
        if (skills.Count < 5)
            response.Improvements.Add($"Adicione mais habilidades técnicas (atual: {skills.Count}, recomendado: 5+)");
        if (educations.Count == 0)
            response.Improvements.Add("Adicione sua formação acadêmica");
        if (string.IsNullOrWhiteSpace(person?.ProfessionalSummary))
            response.Improvements.Add("Adicione um resumo profissional");
        if (string.IsNullOrWhiteSpace(person?.PhotoUrl))
            response.Improvements.Add("Adicione uma foto de perfil");
        if (string.IsNullOrWhiteSpace(person?.Phone))
            response.Improvements.Add("Adicione um telefone de contato");

        // Recomendações
        if (person?.CalculateCompletionPercentage() < 60)
            response.Recommendations.Add("Complete seu perfil para atingir 60% e poder publicar o currículo");
        if (skills.Any() && skills.All(s => s.Category != SkillCategory.SoftSkills))
            response.Recommendations.Add("Adicione soft skills (comunicação, liderança, trabalho em equipe)");
        if (skills.Any() && skills.All(s => s.Category != SkillCategory.CloudDevOps))
            response.Recommendations.Add("Considere adicionar habilidades de Cloud/DevOps");
        if (experiences.Any() && experiences.All(e => string.IsNullOrWhiteSpace(e.Description)))
            response.Recommendations.Add("Adicione descrições detalhadas nas experiências profissionais");

        if (response.Improvements.Count == 0)
            response.Improvements.Add("Perfil excelente! Continue atualizando regularmente.");

        return response;
    }
}

public class GetViewsChartHandler : IRequestHandler<GetViewsChartQuery, ViewsChartResponse>
{
    private readonly IResumeViewRepository _viewRepo;
    private readonly ICurrentUserService _currentUser;

    public GetViewsChartHandler(IResumeViewRepository viewRepo, ICurrentUserService currentUser)
    {
        _viewRepo = viewRepo; _currentUser = currentUser;
    }

    public async Task<ViewsChartResponse> Handle(GetViewsChartQuery request, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var since = DateTime.UtcNow.AddDays(-request.Days);

        var views = await _viewRepo.FindAsync(
            v => v.PersonId == personId && v.CreatedAt >= since, ct);

        var dataPoints = views
            .GroupBy(v => v.CreatedAt.Date)
            .Select(g => new ViewsDataPoint
            {
                Period = g.Key.ToString("dd/MM"),
                Views = g.Count(),
                Downloads = g.Count(v => v.PdfDownloaded)
            })
            .OrderBy(d => d.Period)
            .ToList();

        return new ViewsChartResponse { DataPoints = dataPoints };
    }
}

public class GetSkillsGapHandler : IRequestHandler<GetSkillsGapQuery, SkillsGapResponse>
{
    private readonly ISkillRepository _skillRepo;
    private readonly IExperienceRepository _experienceRepo;
    private readonly ICurrentUserService _currentUser;

    private static readonly Dictionary<SkillCategory, List<string>> RecommendedSkillsByCategory = new()
    {
        { SkillCategory.Backend, new() { "C#", ".NET", "ASP.NET Core", "Entity Framework", "SQL", "REST APIs" } },
        { SkillCategory.Frontend, new() { "React", "TypeScript", "HTML5", "CSS3", "Angular" } },
        { SkillCategory.Database, new() { "PostgreSQL", "MongoDB", "Redis", "SQL Server" } },
        { SkillCategory.CloudDevOps, new() { "Docker", "Kubernetes", "AWS", "Azure", "CI/CD" } },
        { SkillCategory.SoftSkills, new() { "Comunicação", "Liderança", "Trabalho em Equipe", "Gestão de Tempo" } }
    };

    public GetSkillsGapHandler(ISkillRepository skillRepo, IExperienceRepository experienceRepo, ICurrentUserService currentUser)
    {
        _skillRepo = skillRepo; _experienceRepo = experienceRepo; _currentUser = currentUser;
    }

    public async Task<SkillsGapResponse> Handle(GetSkillsGapQuery request, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var skills = await _skillRepo.GetByPersonIdAsync(personId, ct);

        var presentSkillNames = skills.Select(s => s.Name.ToLowerInvariant()).ToHashSet();
        var primaryCategory = skills
            .GroupBy(s => s.Category)
            .OrderByDescending(g => g.Count())
            .Select(g => (SkillCategory?)g.Key)
            .FirstOrDefault();

        var recommendedSkills = new List<string>();

        if (primaryCategory.HasValue && RecommendedSkillsByCategory.TryGetValue(primaryCategory.Value, out var categorySkills))
        {
            recommendedSkills = categorySkills
                .Where(s => !presentSkillNames.Contains(s.ToLowerInvariant()))
                .ToList();
        }

        // Adiciona recomendações de soft skills se não tiver
        if (!skills.Any(s => s.Category == SkillCategory.SoftSkills))
        {
            recommendedSkills.AddRange(RecommendedSkillsByCategory[SkillCategory.SoftSkills].Take(2));
        }

        return new SkillsGapResponse
        {
            PresentSkills = skills.Select(s => s.Name).ToList(),
            RecommendedSkills = recommendedSkills.Distinct().Take(5).ToList(),
            PrimaryCategory = primaryCategory?.GetDisplayName()
        };
    }
}
