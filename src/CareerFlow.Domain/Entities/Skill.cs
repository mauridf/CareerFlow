using CareerFlow.Domain.Common;
using CareerFlow.Domain.Enums;

namespace CareerFlow.Domain.Entities;

public class Skill : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillType Type { get; set; } = SkillType.TOOLS;
    public SkillLevel Level { get; set; } = SkillLevel.BASIC;

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<ProfessionalExperience> ProfessionalExperiences { get; set; } = new List<ProfessionalExperience>();
}