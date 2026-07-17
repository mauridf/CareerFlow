using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Skills.DTOs;

public record CreateSkillRequest(
    string Name,
    SkillCategory Category,
    ProficiencyLevel ProficiencyLevel = ProficiencyLevel.Basic,
    bool IsPrimary = false,
    int DisplayOrder = 0);

public record UpdateSkillRequest(
    string Name,
    SkillCategory Category,
    ProficiencyLevel ProficiencyLevel,
    bool IsPrimary,
    int DisplayOrder);

public record ReorderSkillsRequest(
    List<SkillOrderItem> Skills);

public record SkillOrderItem(
    Guid Id,
    int DisplayOrder);

public class SkillResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ProficiencyLevel { get; set; } = string.Empty;
    public int ProficiencyScore { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SkillCategoryResponse
{
    public SkillCategory Value { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
