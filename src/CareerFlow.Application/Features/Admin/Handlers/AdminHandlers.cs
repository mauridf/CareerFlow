using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Admin.Commands;
using CareerFlow.Application.Features.Admin.DTOs;
using CareerFlow.Application.Features.Admin.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Admin.Handlers;

public class GetSystemStatsHandler : IRequestHandler<GetSystemStatsQuery, AdminSystemStatsResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IPersonRepository _personRepo;
    private readonly IResumeViewRepository _viewRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ILogger<GetSystemStatsHandler> _logger;

    public GetSystemStatsHandler(
        IUserRepository userRepo, IPersonRepository personRepo,
        IResumeViewRepository viewRepo, IResumeAnalyticsRepository analyticsRepo,
        ILogger<GetSystemStatsHandler> logger)
    {
        _userRepo = userRepo; _personRepo = personRepo;
        _viewRepo = viewRepo; _analyticsRepo = analyticsRepo;
        _logger = logger;
    }

    public async Task<AdminSystemStatsResponse> Handle(GetSystemStatsQuery req, CancellationToken ct)
    {
        var users = await _userRepo.GetAllAsync(ct);
        var analytics = await _analyticsRepo.GetAllAsync(ct);
        var views = await _viewRepo.GetAllAsync(ct);

        _logger.LogInformation("📊 Estatísticas do sistema geradas por admin");

        return new AdminSystemStatsResponse
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive && !u.DeletedAt.HasValue),
            PremiumUsers = users.Count(u => u.IsPremium),
            AdminUsers = users.Count(u => u.Role == UserRole.Admin),
            TotalResumes = analytics.Count,
            PublishedResumes = analytics.Count(a => a.Status == ResumeStatus.Published),
            TotalResumeViews = views.Count,
            AverageAtsScore = analytics.Any(a => a.AtsScore.HasValue)
                ? Math.Round(analytics.Where(a => a.AtsScore.HasValue).Average(a => a.AtsScore!.Value), 1)
                : 0
        };
    }
}

public class GetUsersListHandler : IRequestHandler<GetUsersListQuery, AdminUsersListResponse>
{
    private readonly IUserRepository _userRepo;

    public GetUsersListHandler(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<AdminUsersListResponse> Handle(GetUsersListQuery req, CancellationToken ct)
    {
        var (users, total) = await _userRepo.FindPaginatedAsync(
            string.IsNullOrWhiteSpace(req.Search) ? null :
                u => u.Name.Contains(req.Search) || u.Email.Contains(req.Search),
            req.Page, req.PageSize, u => u.CreatedAt, true, ct);

        return new AdminUsersListResponse
        {
            Users = users.Select(u => new AdminUserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.GetDisplayName(),
                IsActive = u.IsActive,
                IsPremium = u.IsPremium,
                EmailVerified = u.IsEmailVerified(),
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt,
                DeletedAt = u.DeletedAt
            }).ToList().AsReadOnly(),
            Total = total,
            Page = req.Page,
            PageSize = req.PageSize
        };
    }
}

public class GetUserDetailHandler : IRequestHandler<GetUserDetailQuery, AdminUserDetailResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IPersonRepository _personRepo;

    public GetUserDetailHandler(IUserRepository userRepo, IPersonRepository personRepo)
    {
        _userRepo = userRepo; _personRepo = personRepo;
    }

    public async Task<AdminUserDetailResponse> Handle(GetUserDetailQuery req, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdWithPersonAsync(req.Id, ct)
            ?? throw new NotFoundException("Usuário", req.Id);

        return new AdminUserDetailResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.GetDisplayName(),
            IsActive = user.IsActive,
            IsPremium = user.IsPremium,
            PremiumUntil = user.PremiumUntil,
            EmailVerified = user.IsEmailVerified(),
            LastLoginAt = user.LastLoginAt,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LockedUntil = user.LockedUntil,
            CreatedAt = user.CreatedAt,
            DeletedAt = user.DeletedAt,
            HasProfile = user.Person != null
        };
    }
}

public class AdminUpdateUserHandler : IRequestHandler<AdminUpdateUserCommand, AdminUserDetailResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;

    public AdminUpdateUserHandler(IUserRepository userRepo, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo; _unitOfWork = unitOfWork;
    }

    public async Task<AdminUserDetailResponse> Handle(AdminUpdateUserCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Usuário", cmd.Id);

        if (!string.IsNullOrWhiteSpace(cmd.Name))
            user.UpdateName(cmd.Name);

        if (!string.IsNullOrWhiteSpace(cmd.Role) && Enum.TryParse<UserRole>(cmd.Role, out var role))
            user.SetRole(role);

        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AdminUserDetailResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.GetDisplayName(),
            IsActive = user.IsActive,
            IsPremium = user.IsPremium,
            EmailVerified = user.IsEmailVerified()
        };
    }
}

public class AdminToggleUserStatusHandler : IRequestHandler<AdminToggleUserStatusCommand, AdminUserDetailResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;

    public AdminToggleUserStatusHandler(IUserRepository userRepo, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo; _unitOfWork = unitOfWork;
    }

    public async Task<AdminUserDetailResponse> Handle(AdminToggleUserStatusCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Usuário", cmd.Id);

        if (user.IsActive)
            user.Deactivate();
        else
            user.Activate();

        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AdminUserDetailResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.GetDisplayName(),
            IsActive = user.IsActive
        };
    }
}

public class AdminDeleteUserHandler : IRequestHandler<AdminDeleteUserCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;

    public AdminDeleteUserHandler(IUserRepository userRepo, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo; _unitOfWork = unitOfWork;
    }

    public async Task Handle(AdminDeleteUserCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Usuário", cmd.Id);

        user.SoftDelete();
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class GetAdminUsersStatsHandler : IRequestHandler<GetAdminUsersStatsQuery, AdminUsersStatsResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly ILogger<GetAdminUsersStatsHandler> _logger;

    public GetAdminUsersStatsHandler(IUserRepository userRepo, ILogger<GetAdminUsersStatsHandler> logger)
    {
        _userRepo = userRepo;
        _logger = logger;
    }

    public async Task<AdminUsersStatsResponse> Handle(GetAdminUsersStatsQuery req, CancellationToken ct)
    {
        var users = await _userRepo.GetAllAsync(ct);

        _logger.LogInformation("📊 Estatísticas de usuários solicitadas por admin");

        return new AdminUsersStatsResponse
        {
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive && !u.DeletedAt.HasValue),
            InactiveUsers = users.Count(u => !u.IsActive && !u.DeletedAt.HasValue),
            PremiumUsers = users.Count(u => u.IsPremium),
            AdminUsers = users.Count(u => u.Role == UserRole.Admin),
            EmailVerified = users.Count(u => u.IsEmailVerified()),
            EmailUnverified = users.Count(u => !u.IsEmailVerified()),
            LockedUsers = users.Count(u => u.LockedUntil.HasValue && u.LockedUntil > DateTime.UtcNow),
            DeletedUsers = users.Count(u => u.DeletedAt.HasValue)
        };
    }
}

public class GetAdminResumesStatsHandler : IRequestHandler<GetAdminResumesStatsQuery, AdminResumesStatsResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ILogger<GetAdminResumesStatsHandler> _logger;

    public GetAdminResumesStatsHandler(
        IPersonRepository personRepo,
        IResumeAnalyticsRepository analyticsRepo,
        ILogger<GetAdminResumesStatsHandler> logger)
    {
        _personRepo = personRepo;
        _analyticsRepo = analyticsRepo;
        _logger = logger;
    }

    public async Task<AdminResumesStatsResponse> Handle(GetAdminResumesStatsQuery req, CancellationToken ct)
    {
        var analytics = await _analyticsRepo.GetAllAsync(ct);
        var persons = await _personRepo.GetAllWithBasicDetailsAsync(ct);

        _logger.LogInformation("📊 Estatísticas de currículos solicitadas por admin");

        var withExperience = persons.Count(p => p.Experiences.Any());
        var withEducation = persons.Count(p => p.Educations.Any());
        var withSkills = persons.Count(p => p.Skills.Any());
        var avgCompletion = persons.Any()
            ? (int)persons.Average(p => p.CalculateCompletionPercentage())
            : 0;

        return new AdminResumesStatsResponse
        {
            TotalResumes = analytics.Count,
            PublishedResumes = analytics.Count(a => a.Status == ResumeStatus.Published),
            DraftResumes = analytics.Count(a => a.Status == ResumeStatus.Draft),
            ArchivedResumes = analytics.Count(a => a.Status == ResumeStatus.Archived),
            AverageCompletionPercentage = avgCompletion,
            ResumesWithExperience = withExperience,
            ResumesWithEducation = withEducation,
            ResumesWithSkills = withSkills
        };
    }
}

public class GetAdminViewsStatsHandler : IRequestHandler<GetAdminViewsStatsQuery, AdminViewsStatsResponse>
{
    private readonly IResumeViewRepository _viewRepo;
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ILogger<GetAdminViewsStatsHandler> _logger;

    public GetAdminViewsStatsHandler(
        IResumeViewRepository viewRepo,
        IResumeAnalyticsRepository analyticsRepo,
        ILogger<GetAdminViewsStatsHandler> logger)
    {
        _viewRepo = viewRepo;
        _analyticsRepo = analyticsRepo;
        _logger = logger;
    }

    public async Task<AdminViewsStatsResponse> Handle(GetAdminViewsStatsQuery req, CancellationToken ct)
    {
        var views = await _viewRepo.GetAllAsync(ct);
        var analytics = await _analyticsRepo.GetAllAsync(ct);
        var now = DateTime.UtcNow;

        _logger.LogInformation("📊 Estatísticas de visualizações solicitadas por admin");

        var viewsBySource = views
            .GroupBy(v => v.Source ?? "direct")
            .ToDictionary(g => g.Key, g => g.Count());

        return new AdminViewsStatsResponse
        {
            TotalViews = views.Count,
            UniqueViews = analytics.Sum(a => a.UniqueViews),
            PdfDownloads = analytics.Sum(a => a.PdfDownloads),
            AverageViewsPerResume = analytics.Any()
                ? Math.Round(analytics.Average(a => a.TotalViews), 1)
                : 0,
            ViewsBySource = viewsBySource,
            ViewsToday = views.Count(v => v.CreatedAt.Date == now.Date),
            ViewsThisWeek = views.Count(v => v.CreatedAt >= now.AddDays(-7)),
            ViewsThisMonth = views.Count(v => v.CreatedAt >= now.AddMonths(-1))
        };
    }
}

public class GetAdminAtsScoresStatsHandler : IRequestHandler<GetAdminAtsScoresStatsQuery, AdminAtsScoresStatsResponse>
{
    private readonly IResumeAnalyticsRepository _analyticsRepo;
    private readonly ILogger<GetAdminAtsScoresStatsHandler> _logger;

    public GetAdminAtsScoresStatsHandler(
        IResumeAnalyticsRepository analyticsRepo,
        ILogger<GetAdminAtsScoresStatsHandler> logger)
    {
        _analyticsRepo = analyticsRepo;
        _logger = logger;
    }

    public async Task<AdminAtsScoresStatsResponse> Handle(GetAdminAtsScoresStatsQuery req, CancellationToken ct)
    {
        var analytics = await _analyticsRepo.GetAllAsync(ct);
        var withScores = analytics.Where(a => a.AtsScore.HasValue).ToList();

        _logger.LogInformation("📊 Estatísticas de scores ATS solicitadas por admin");

        if (withScores.Count == 0)
        {
            return new AdminAtsScoresStatsResponse();
        }

        var scores = withScores.Select(a => a.AtsScore!.Value).OrderBy(s => s).ToList();

        var distribution = new Dictionary<string, int>
        {
            ["0-20"] = scores.Count(s => s <= 20),
            ["21-40"] = scores.Count(s => s > 20 && s <= 40),
            ["41-60"] = scores.Count(s => s > 40 && s <= 60),
            ["61-80"] = scores.Count(s => s > 60 && s <= 80),
            ["81-100"] = scores.Count(s => s > 80)
        };

        var mid = scores.Count / 2;
        var median = scores.Count % 2 != 0
            ? scores[mid]
            : (scores[mid - 1] + scores[mid]) / 2.0;

        return new AdminAtsScoresStatsResponse
        {
            AverageScore = Math.Round(scores.Average(), 1),
            MedianScore = Math.Round(median, 1),
            MinScore = scores.Min(),
            MaxScore = scores.Max(),
            TotalAnalyzed = withScores.Count,
            ScoreDistribution = distribution
        };
    }
}

public class AdminManagePremiumHandler : IRequestHandler<AdminManagePremiumCommand, AdminUserDetailResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminManagePremiumHandler> _logger;

    public AdminManagePremiumHandler(
        IUserRepository userRepo,
        IUnitOfWork unitOfWork,
        ILogger<AdminManagePremiumHandler> logger)
    {
        _userRepo = userRepo; _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<AdminUserDetailResponse> Handle(AdminManagePremiumCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Usuário", cmd.Id);

        if (cmd.Activate)
        {
            var until = cmd.Until ?? DateTime.UtcNow.AddYears(1);
            user.ActivatePremium(until);
            _logger.LogInformation("✅ Premium ativado para usuário {UserId} até {Until}", user.Id, until);
        }
        else
        {
            user.DeactivatePremium();
            _logger.LogInformation("✅ Premium desativado para usuário {UserId}", user.Id);
        }

        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AdminUserDetailResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.GetDisplayName(),
            IsActive = user.IsActive,
            IsPremium = user.IsPremium,
            PremiumUntil = user.PremiumUntil,
            EmailVerified = user.IsEmailVerified()
        };
    }
}
