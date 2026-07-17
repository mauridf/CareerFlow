using Microsoft.AspNetCore.Http;

namespace CareerFlow.Application.Features.Profile.DTOs;

// ============================================
// Request DTOs
// ============================================

public record CreateProfileRequest(
    string? Phone = null,
    string? City = null,
    string? State = null,
    DateTime? BirthDate = null,
    string? ProfessionalSummary = null,
    string? CurrentPosition = null,
    string? CurrentCompany = null);

public record UpdateProfileRequest(
    string? Phone = null,
    string? City = null,
    string? State = null,
    DateTime? BirthDate = null,
    string? ProfessionalSummary = null,
    string? CurrentPosition = null,
    string? CurrentCompany = null);

public record UpdateProfilePhotoRequest(
    IFormFile Photo);

// ============================================
// Response DTOs
// ============================================

public class ProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ProfessionalSummary { get; set; }
    public string? PhotoUrl { get; set; }
    public string? CurrentPosition { get; set; }
    public string? CurrentCompany { get; set; }
    public bool IsPublic { get; set; }
    public string? ResumeSlug { get; set; }
    public int CompletionPercentage { get; set; }
    public bool CanGenerateResume { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProfileCompletionResponse
{
    public int Percentage { get; set; }
    public bool CanGenerateResume { get; set; }
    public List<string> MissingFields { get; set; } = new();
    public List<string> CompletedFields { get; set; } = new();
}
