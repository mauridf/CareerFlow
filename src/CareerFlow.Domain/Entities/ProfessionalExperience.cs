using System.ComponentModel.DataAnnotations.Schema;
using CareerFlow.Domain.Common;

namespace CareerFlow.Domain.Entities;

public class ProfessionalExperience : BaseEntity
{
    public Guid UserId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Responsibilities { get; set; } = string.Empty;
    public bool IsPaid { get; set; } = true;

    // Computed property: IsCurrent
    public bool IsCurrent => !EndDate.HasValue || EndDate > DateTime.UtcNow;

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<SkillExperience> SkillExperiences { get; set; } = new List<SkillExperience>();
    [NotMapped] // Não mapear para o banco
    public IEnumerable<Skill> Skills => SkillExperiences?.Select(se => se.Skill) ?? Enumerable.Empty<Skill>();
}