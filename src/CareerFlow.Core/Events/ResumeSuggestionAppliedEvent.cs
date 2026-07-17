namespace CareerFlow.Core.Events;

public sealed class ResumeSuggestionAppliedEvent : DomainEvent
{
    public Guid SuggestionId { get; }
    public Guid PersonId { get; }

    public ResumeSuggestionAppliedEvent(Guid suggestionId, Guid personId)
    {
        SuggestionId = suggestionId;
        PersonId = personId;
    }
}
