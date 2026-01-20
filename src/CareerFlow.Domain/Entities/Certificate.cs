using CareerFlow.Domain.Common;

namespace CareerFlow.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CertificatePath { get; set; }

    // Computed property: IsValid
    public bool IsValid => !EndDate.HasValue || EndDate > DateTime.UtcNow;

    // Navigation Property
    public User User { get; set; } = null!;
}