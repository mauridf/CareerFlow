using System.ComponentModel.DataAnnotations;

namespace CareerFlow.Application.DTOs;

public class ProfessionalExperienceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Responsibilities { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public bool IsCurrent { get; set; }
    public List<SkillDto> Skills { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProfessionalExperienceDto
{
    [Required]
    [StringLength(200)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Position { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    [StringLength(2000)]
    public string Responsibilities { get; set; } = string.Empty;

    public bool IsPaid { get; set; } = true;

    public List<Guid> SkillIds { get; set; } = new();
}

public class UpdateProfessionalExperienceDto
{
    [StringLength(200)]
    public string? Company { get; set; }

    [StringLength(200)]
    public string? Position { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(2000)]
    public string? Responsibilities { get; set; }

    public bool? IsPaid { get; set; }

    public List<Guid>? SkillIds { get; set; }
}