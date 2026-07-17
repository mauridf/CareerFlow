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
