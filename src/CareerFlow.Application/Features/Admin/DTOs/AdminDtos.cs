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

public class AdminUsersStatsResponse
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int PremiumUsers { get; set; }
    public int AdminUsers { get; set; }
    public int EmailVerified { get; set; }
    public int EmailUnverified { get; set; }
    public int LockedUsers { get; set; }
    public int DeletedUsers { get; set; }
}

public class AdminResumesStatsResponse
{
    public int TotalResumes { get; set; }
    public int PublishedResumes { get; set; }
    public int DraftResumes { get; set; }
    public int ArchivedResumes { get; set; }
    public int AverageCompletionPercentage { get; set; }
    public int ResumesWithExperience { get; set; }
    public int ResumesWithEducation { get; set; }
    public int ResumesWithSkills { get; set; }
}

public class AdminViewsStatsResponse
{
    public int TotalViews { get; set; }
    public int UniqueViews { get; set; }
    public int PdfDownloads { get; set; }
    public double AverageViewsPerResume { get; set; }
    public IReadOnlyDictionary<string, int> ViewsBySource { get; set; } = new Dictionary<string, int>();
    public int ViewsToday { get; set; }
    public int ViewsThisWeek { get; set; }
    public int ViewsThisMonth { get; set; }
}

public class AdminAtsScoresStatsResponse
{
    public double AverageScore { get; set; }
    public double MedianScore { get; set; }
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    public int TotalAnalyzed { get; set; }
    public IReadOnlyDictionary<string, int> ScoreDistribution { get; set; } = new Dictionary<string, int>();
}
