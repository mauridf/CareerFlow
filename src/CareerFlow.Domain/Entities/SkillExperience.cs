using CareerFlow.Domain.Common;

namespace CareerFlow.Domain.Entities;

public class SkillExperience : BaseEntity
{
    public Guid SkillId { get; set; }
    public Guid ProfessionalExperienceId { get; set; }

    // Navigation Properties
    public Skill Skill { get; set; } = null!;
    public ProfessionalExperience ProfessionalExperience { get; set; } = null!;
}