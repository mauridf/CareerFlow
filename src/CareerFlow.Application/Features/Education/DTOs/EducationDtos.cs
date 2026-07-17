using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Education.DTOs;

public record CreateEducationRequest(
    string Institution,
    string Course,
    EducationLevel EducationLevel,
    DateTime StartDate,
    DateTime? EndDate = null,
    EducationStatus? Status = null,
    string? Description = null,
    string? Grade = null,
    string? ThesisTitle = null);

public record UpdateEducationRequest(
    string Institution,
    string Course,
    EducationLevel EducationLevel,
    DateTime StartDate,
    DateTime? EndDate = null,
    EducationStatus Status = EducationStatus.Completed,
    string? Description = null,
    string? Grade = null,
    string? ThesisTitle = null);

public class EducationResponse
{
    public Guid Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Grade { get; set; }
    public string? ThesisTitle { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
