using CareerFlow.Domain.Common;
using CareerFlow.Domain.Enums;

namespace CareerFlow.Domain.Entities;

public class Language : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public LanguageLevel Level { get; set; } = LanguageLevel.BASIC;

    // Navigation Property
    public User User { get; set; } = null!;
}