using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Experiences.DTOs;

public record CreateExperienceRequest(
    string CompanyName,
    string Position,
    DateTime StartDate,
    DateTime? EndDate = null,
    string? Description = null,
    List<Guid>? SkillsUsed = null,
    string? City = null,
    string? State = null,
    string? Country = null,
    EmploymentType? EmploymentType = null);

public record UpdateExperienceRequest(
    string CompanyName,
    string Position,
    DateTime StartDate,
    DateTime? EndDate = null,
    string? Description = null,
    List<Guid>? SkillsUsed = null,
    string? City = null,
    string? State = null,
    EmploymentType? EmploymentType = null);

public class ExperienceResponse
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public List<Guid> SkillsUsed { get; set; } = new();
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? EmploymentType { get; set; }
    public string DurationFormatted { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
