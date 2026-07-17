using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa idiomas do usuário.
/// </summary>
public class Language : AggregateRoot<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public string LanguageName { get; private set; } = string.Empty;
    public LanguageLevel ProficiencyLevel { get; private set; } = LanguageLevel.A1;
    public bool IsNative { get; private set; }
    public string? ReadingLevel { get; private set; }
    public string? WritingLevel { get; private set; }
    public string? ListeningLevel { get; private set; }
    public string? SpeakingLevel { get; private set; }
    public int DisplayOrder { get; private set; }

    private Language() { }

    public static Language Create(
        Guid personId,
        string languageName,
        LanguageLevel level,
        bool isNative = false,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(languageName))
            throw new ArgumentException("Nome do idioma é obrigatório", nameof(languageName));

        var language = new Language
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            LanguageName = languageName.Trim(),
            ProficiencyLevel = level,
            IsNative = isNative,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        language.AddDomainEvent(new LanguageCreatedEvent(language.Id, personId, languageName.Trim(), level));

        return language;
    }

    public void Update(string languageName, LanguageLevel level, bool isNative)
    {
        LanguageName = languageName.Trim();
        ProficiencyLevel = level;
        IsNative = isNative;
        MarkAsUpdated();
        AddDomainEvent(new LanguageUpdatedEvent(Id, PersonId));
    }
}
