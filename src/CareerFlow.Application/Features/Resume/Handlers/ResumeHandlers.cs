using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Resume.Commands;
using CareerFlow.Application.Features.Resume.DTOs;
using CareerFlow.Application.Features.Resume.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Interfaces.Settings;
using Microsoft.Extensions.Options;

namespace CareerFlow.Application.Features.Resume.Handlers;

public class GetResumeHandler : IRequestHandler<GetResumeQuery, ResumeResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly ICurrentUserService _currentUser;

    public GetResumeHandler(IPersonRepository personRepo, ICurrentUserService currentUser)
    {
        _personRepo = personRepo; _currentUser = currentUser;
    }

    public async Task<ResumeResponse> Handle(GetResumeQuery req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetFullProfileAsync(personId, ct)
            ?? throw new NotFoundException("Perfil");
        return ResumeMapper.MapToResume(person);
    }

    private static ResumeResponse MapToResume(Person p) => new()
    {
        Person = new PersonInfo
        {
            Name = p.User?.Name ?? "",
            Email = p.User?.Email ?? "",
            Phone = p.Phone,
            City = p.City,
            State = p.State,
            ProfessionalSummary = p.ProfessionalSummary,
            PhotoUrl = p.PhotoUrl,
            CurrentPosition = p.CurrentPosition,
            CurrentCompany = p.CurrentCompany,
            ResumeSlug = p.ResumeSlug
        },
        Experiences = p.Experiences.Select(e => new ExperienceInfo
        {
            CompanyName = e.CompanyName,
            Position = e.Position,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsCurrent = e.IsCurrent,
            Description = e.Description,
            EmploymentType = e.EmploymentType?.GetDisplayName(),
            DurationFormatted = e.GetFormattedDuration()
        }).ToList(),
        Educations = p.Educations.Select(e => new EducationInfo
        {
            Institution = e.Institution,
            Course = e.Course,
            Level = e.EducationLevel.GetDisplayName(),
            Status = e.Status.GetDisplayName(),
            StartDate = e.StartDate,
            EndDate = e.EndDate
        }).ToList(),
        Skills = p.Skills.Select(s => new SkillInfo
        {
            Name = s.Name,
            Category = s.Category.GetDisplayName(),
            Level = s.ProficiencyLevel.GetDisplayName(),
            Score = s.ProficiencyLevel.GetScore(),
            IsPrimary = s.IsPrimary
        }).ToList(),
        Languages = p.Languages.Select(l => new LanguageInfo
        {
            LanguageName = l.LanguageName,
            Level = l.ProficiencyLevel.GetDisplayName(),
            IsNative = l.IsNative
        }).ToList(),
        Certificates = p.Certificates.Select(c => new CertificateInfo
        {
            Title = c.Title,
            Issuer = c.Issuer,
            IssueDate = c.IssueDate,
            ExpirationDate = c.ExpirationDate,
            CredentialUrl = c.CredentialUrl
        }).ToList(),
        SocialNetworks = p.SocialNetworks.Select(s => new SocialNetworkInfo
        {
            NetworkType = s.NetworkType.GetDisplayName(),
            Url = s.Url
        }).ToList()
    };
}

public class GetPublicResumeHandler : IRequestHandler<GetPublicResumeQuery, ResumeResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeViewRepository _viewRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly IUnitOfWork _unitOfWork;

    public GetPublicResumeHandler(
        IPersonRepository personRepo, IResumeViewRepository viewRepo,
        IResumeAnalyticsRepository analyticsRepo, IUnitOfWork unitOfWork)
    {
        _personRepo = personRepo; _viewRepo = viewRepo;
        _analyticsRepo = analyticsRepo; _unitOfWork = unitOfWork;
    }

    public async Task<ResumeResponse> Handle(GetPublicResumeQuery req, CancellationToken ct)
    {
        var person = await _personRepo.GetBySlugAsync(req.Slug, ct)
            ?? throw new NotFoundException("Currículo público não encontrado");

        // Registra visualização
        var view = ResumeView.Record(person.Id, null, null, null, "direct");
        await _viewRepo.AddAsync(view, ct);

        var analytics = await _analyticsRepo.GetByPersonIdAsync(person.Id, ct);
        if (analytics != null)
        {
            analytics.IncrementView();
            _analyticsRepo.Update(analytics);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return ResumeMapper.MapToResume(person);
    }
}

public class GetResumeAnalyticsHandler : IRequestHandler<GetResumeAnalyticsQuery, ResumeAnalyticsResponse>
{
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly IResumeViewRepository _viewRepo;
    private readonly IPersonRepository _personRepo;
    private readonly ICurrentUserService _currentUser;

    public GetResumeAnalyticsHandler(
        IResumeAnalyticsRepository analyticsRepo, IResumeViewRepository viewRepo,
        IPersonRepository personRepo, ICurrentUserService currentUser)
    {
        _analyticsRepo = analyticsRepo; _viewRepo = viewRepo;
        _personRepo = personRepo; _currentUser = currentUser;
    }

    public async Task<ResumeAnalyticsResponse> Handle(GetResumeAnalyticsQuery req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct);
        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);

        if (analytics == null)
            return new ResumeAnalyticsResponse();

        var totalViews = await _viewRepo.CountByPersonIdAsync(personId, ct);
        var uniqueViews = await _viewRepo.CountUniqueByPersonIdAsync(personId, ct);
        var pdfDownloads = await _viewRepo.CountPdfDownloadsAsync(personId, ct);

        return new ResumeAnalyticsResponse
        {
            TotalViews = totalViews,
            UniqueViews = uniqueViews,
            PdfDownloads = pdfDownloads,
            SharesCount = analytics.SharesCount,
            AtsScore = analytics.AtsScore,
            AtsCompatibility = analytics.AtsCompatibility,
            AtsIssues = analytics.AtsIssues,
            AtsSuggestions = analytics.AtsSuggestions,
            LastViewedAt = analytics.LastViewedAt,
            Status = analytics.Status.GetDisplayName(),
            CompletionPercentage = person?.CalculateCompletionPercentage() ?? 0
        };
    }
}

public class ShareResumeHandler : IRequestHandler<ShareResumeCommand, string>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ShareResumeHandler(
        IPersonRepository personRepo, IResumeAnalyticsRepository analyticsRepo,
        ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _personRepo = personRepo; _analyticsRepo = analyticsRepo;
        _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(ShareResumeCommand req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct)
            ?? throw new NotFoundException("Perfil");

        if (string.IsNullOrWhiteSpace(person.ResumeSlug))
        {
            var userName = _currentUser.Name ?? "user";
            person.SetResumeSlug(person.GenerateSlug(userName));
            _personRepo.Update(person);
        }

        person.SetPublic(true);

        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);
        if (analytics != null)
        {
            analytics.IncrementShare();
            analytics.Publish();
            _analyticsRepo.Update(analytics);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return $"http://localhost:5000/api/v1/resume/shared/{person.ResumeSlug}";
    }
}

public class PublishResumeHandler : IRequestHandler<PublishResumeCommand>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public PublishResumeHandler(
        IPersonRepository personRepo, IResumeAnalyticsRepository analyticsRepo,
        ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _personRepo = personRepo; _analyticsRepo = analyticsRepo;
        _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task Handle(PublishResumeCommand req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct)
            ?? throw new NotFoundException("Perfil");

        if (!person.CanGenerateResume())
            throw new DomainException("Perfil não atinge os requisitos mínimos para publicação (60% completo)");

        person.SetPublic(true);

        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);
        if (analytics != null)
        {
            analytics.Publish();
            _analyticsRepo.Update(analytics);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class GenerateResumeHandler : IRequestHandler<GenerateResumeCommand, byte[]>
{
    private readonly IPersonRepository _personRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IPdfGeneratorService _pdfGenerator;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ILogger<GenerateResumeHandler> _logger;

    public GenerateResumeHandler(
        IPersonRepository personRepo,
        ICurrentUserService currentUser,
        IPdfGeneratorService pdfGenerator,
        IResumeAnalyticsRepository analyticsRepo,
        ILogger<GenerateResumeHandler> logger)
    {
        _personRepo = personRepo;
        _currentUser = currentUser;
        _pdfGenerator = pdfGenerator;
        _analyticsRepo = analyticsRepo;
        _logger = logger;
    }

    public async Task<byte[]> Handle(GenerateResumeCommand req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetFullProfileAsync(personId, ct)
            ?? throw new NotFoundException("Perfil");

        if (!person.CanGenerateResume())
            throw new DomainException("Perfil não atinge os requisitos mínimos para gerar currículo (60% completo)");

        var resumeData = ResumeMapper.MapToResumeData(person);
        var pdf = await _pdfGenerator.GenerateResumePdfAsync(resumeData, ct);

        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);
        if (analytics != null)
        {
            analytics.IncrementPdfDownload();
            _analyticsRepo.Update(analytics);
        }

        _logger.LogInformation("✅ PDF gerado para PersonId={PersonId}", personId);

        return pdf;
    }
}

public class GenerateAtsResumeHandler : IRequestHandler<GenerateAtsResumeCommand, byte[]>
{
    private readonly IPersonRepository _personRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IPdfGeneratorService _pdfGenerator;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ILogger<GenerateAtsResumeHandler> _logger;

    public GenerateAtsResumeHandler(
        IPersonRepository personRepo,
        ICurrentUserService currentUser,
        IPdfGeneratorService pdfGenerator,
        IResumeAnalyticsRepository analyticsRepo,
        ILogger<GenerateAtsResumeHandler> logger)
    {
        _personRepo = personRepo;
        _currentUser = currentUser;
        _pdfGenerator = pdfGenerator;
        _analyticsRepo = analyticsRepo;
        _logger = logger;
    }

    public async Task<byte[]> Handle(GenerateAtsResumeCommand req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetFullProfileAsync(personId, ct)
            ?? throw new NotFoundException("Perfil");

        if (!person.CanGenerateResume())
            throw new DomainException("Perfil não atinge os requisitos mínimos para gerar currículo (60% completo)");

        var resumeData = ResumeMapper.MapToResumeData(person);
        var pdf = await _pdfGenerator.GenerateAtsResumePdfAsync(resumeData, ct);

        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);
        if (analytics != null)
        {
            analytics.IncrementPdfDownload();
            _analyticsRepo.Update(analytics);
        }

        _logger.LogInformation("✅ PDF ATS gerado para PersonId={PersonId}", personId);

        return pdf;
    }
}

public class AnalyzeResumeHandler : IRequestHandler<AnalyzeResumeCommand, ResumeAnalyticsResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly IResumeViewRepository _viewRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AnalyzeResumeHandler> _logger;

    public AnalyzeResumeHandler(
        IPersonRepository personRepo,
        IResumeAnalyticsRepository analyticsRepo,
        IResumeViewRepository viewRepo,
        ICurrentUserService currentUser,
        ILogger<AnalyzeResumeHandler> logger)
    {
        _personRepo = personRepo;
        _analyticsRepo = analyticsRepo;
        _viewRepo = viewRepo;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ResumeAnalyticsResponse> Handle(AnalyzeResumeCommand req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct);
        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);

        if (analytics == null)
            return new ResumeAnalyticsResponse();

        var totalViews = await _viewRepo.CountByPersonIdAsync(personId, ct);
        var uniqueViews = await _viewRepo.CountUniqueByPersonIdAsync(personId, ct);
        var pdfDownloads = await _viewRepo.CountPdfDownloadsAsync(personId, ct);

        _logger.LogInformation("✅ Análise do currículo concluída para PersonId={PersonId}", personId);

        return new ResumeAnalyticsResponse
        {
            TotalViews = totalViews,
            UniqueViews = uniqueViews,
            PdfDownloads = pdfDownloads,
            SharesCount = analytics.SharesCount,
            AtsScore = analytics.AtsScore,
            AtsCompatibility = analytics.AtsCompatibility,
            AtsIssues = analytics.AtsIssues,
            AtsSuggestions = analytics.AtsSuggestions,
            LastViewedAt = analytics.LastViewedAt,
            Status = analytics.Status.GetDisplayName(),
            CompletionPercentage = person?.CalculateCompletionPercentage() ?? 0
        };
    }
}

public class UnpublishResumeHandler : IRequestHandler<UnpublishResumeCommand>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UnpublishResumeHandler(
        IPersonRepository personRepo,
        IResumeAnalyticsRepository analyticsRepo,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _personRepo = personRepo;
        _analyticsRepo = analyticsRepo;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UnpublishResumeCommand req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var person = await _personRepo.GetByIdAsync(personId, ct)
            ?? throw new NotFoundException("Perfil");

        person.SetPublic(false);

        var analytics = await _analyticsRepo.GetByPersonIdAsync(personId, ct);
        if (analytics != null)
        {
            analytics.Unpublish();
            _analyticsRepo.Update(analytics);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
