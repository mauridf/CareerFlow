using System.ComponentModel.DataAnnotations;
using CareerFlow.Domain.Enums;

namespace CareerFlow.Application.DTOs;

public class SkillDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSkillDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Level { get; set; } = string.Empty;
}

public class UpdateSkillDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Level { get; set; }
}

public class SkillFilterDto
{
    public string? Type { get; set; }
    public string? Level { get; set; }
}