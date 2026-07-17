using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Events;

public sealed class LanguageCreatedEvent : DomainEvent
{
    public Guid LanguageId { get; }
    public Guid PersonId { get; }
    public string LanguageName { get; }
    public LanguageLevel Level { get; }

    public LanguageCreatedEvent(Guid languageId, Guid personId, string languageName, LanguageLevel level)
    {
        LanguageId = languageId;
        PersonId = personId;
        LanguageName = languageName;
        Level = level;
    }
}
