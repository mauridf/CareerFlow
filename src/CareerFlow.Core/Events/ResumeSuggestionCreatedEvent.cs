namespace CareerFlow.Core.Events;

public sealed class ResumeSuggestionCreatedEvent : DomainEvent
{
    public Guid SuggestionId { get; }
    public Guid PersonId { get; }
    public string Category { get; }
    public string Title { get; }

    public ResumeSuggestionCreatedEvent(Guid suggestionId, Guid personId, string category, string title)
    {
        SuggestionId = suggestionId;
        PersonId = personId;
        Category = category;
        Title = title;
    }
}
