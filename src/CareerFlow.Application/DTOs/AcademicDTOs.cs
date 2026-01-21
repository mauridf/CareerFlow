using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CareerFlow.Application.DTOs;

public class AcademicBackgroundDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? DiplomaPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateAcademicBackgroundDto
{
    [Required]
    [StringLength(200)]
    public string Institution { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    public string Level { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public IFormFile? Diploma { get; set; }
}

public class UpdateAcademicBackgroundDto
{
    [StringLength(200)]
    public string? Institution { get; set; }

    [StringLength(200)]
    public string? CourseName { get; set; }

    public string? Level { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public IFormFile? Diploma { get; set; }
}