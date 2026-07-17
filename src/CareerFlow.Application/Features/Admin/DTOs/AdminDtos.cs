namespace CareerFlow.Application.Features.Admin.DTOs;

public class AdminUserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPremium { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class AdminUserDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPremium { get; set; }
    public DateTime? PremiumUntil { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool HasProfile { get; set; }
}

public class AdminUpdateUserRequest
{
    public string? Name { get; set; }
    public string? Role { get; set; }
}

public class AdminSystemStatsResponse
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int PremiumUsers { get; set; }
    public int AdminUsers { get; set; }
    public int TotalResumes { get; set; }
    public int PublishedResumes { get; set; }
    public int TotalResumeViews { get; set; }
    public double AverageAtsScore { get; set; }
}

public class AdminUsersListResponse
{
    public IReadOnlyList<AdminUserResponse> Users { get; set; } = new List<AdminUserResponse>().AsReadOnly();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
