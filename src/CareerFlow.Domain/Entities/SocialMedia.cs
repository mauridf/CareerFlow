using CareerFlow.Domain.Common;

namespace CareerFlow.Domain.Entities;

public class SocialMedia : BaseEntity
{
    public Guid UserId { get; set; }
    public string Platform { get; set; } = string.Empty; // LinkedIn, GitHub, etc.
    public string Url { get; set; } = string.Empty;

    // Navigation Property
    public User User { get; set; } = null!;
}