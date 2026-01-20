using CareerFlow.Domain.Common;

namespace CareerFlow.Domain.Entities;

public class ProfessionalSummary : BaseEntity
{
    public Guid UserId { get; set; }
    public string Summary { get; set; } = string.Empty;

    // Navigation Property
    public User User { get; set; } = null!;
}