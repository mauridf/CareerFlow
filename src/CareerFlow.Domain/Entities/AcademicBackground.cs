using CareerFlow.Domain.Common;
using CareerFlow.Domain.Enums;

namespace CareerFlow.Domain.Entities;

public class AcademicBackground : BaseEntity
{
    public Guid UserId { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public EducationLevel Level { get; set; } = EducationLevel.GRADUATION;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? DiplomaPath { get; set; }

    // Computed property: IsCurrent
    public bool IsCurrent => !EndDate.HasValue || EndDate > DateTime.UtcNow;

    // Navigation Property
    public User User { get; set; } = null!;
}