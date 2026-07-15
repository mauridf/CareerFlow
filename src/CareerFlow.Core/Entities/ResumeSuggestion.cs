namespace CareerFlow.Core.Entities;

/// <summary>
/// Sugestão de melhoria para o currículo.
/// </summary>
public class ResumeSuggestion : Entity<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public string Category { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Priority { get; private set; } = "medium";
    public bool IsApplied { get; private set; }
    public DateTime? AppliedAt { get; private set; }

    private ResumeSuggestion() { }

    public static ResumeSuggestion Create(
        Guid personId,
        string category,
        string title,
        string? description,
        string priority = "medium")
    {
        return new ResumeSuggestion
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Category = category,
            Title = title,
            Description = description,
            Priority = priority,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsApplied()
    {
        IsApplied = true;
        AppliedAt = DateTime.UtcNow;
    }
}
