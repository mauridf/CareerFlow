using System.ComponentModel.DataAnnotations;

namespace CareerFlow.Application.DTOs;

public class ProfessionalSummaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProfessionalSummaryDto
{
    [Required]
    [StringLength(2000, MinimumLength = 50)]
    public string Summary { get; set; } = string.Empty;
}

public class UpdateProfessionalSummaryDto
{
    [StringLength(2000, MinimumLength = 50)]
    public string? Summary { get; set; }
}