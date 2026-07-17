namespace CareerFlow.Core.Events;

public sealed class LanguageUpdatedEvent : DomainEvent
{
    public Guid LanguageId { get; }
    public Guid PersonId { get; }

    public LanguageUpdatedEvent(Guid languageId, Guid personId)
    {
        LanguageId = languageId;
        PersonId = personId;
    }
}
