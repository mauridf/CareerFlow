using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.Languages.DTOs;

public record CreateLanguageRequest(string LanguageName, LanguageLevel ProficiencyLevel, bool IsNative = false);
public record UpdateLanguageRequest(string LanguageName, LanguageLevel ProficiencyLevel, bool IsNative);

public class LanguageResponse
{
    public Guid Id { get; set; }
    public string LanguageName { get; set; } = string.Empty;
    public string ProficiencyLevel { get; set; } = string.Empty;
    public int ProficiencyScore { get; set; }
    public bool IsNative { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
